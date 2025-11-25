using System.Collections.Generic;
using Spellwright.Components.LLM;
using Unity.Entities;
using UnityEngine;

namespace Spellwright.Tests.Manual
{
    public class LLMTester : MonoBehaviour
    {
        private EntityManager _entityManager;
        private string _prompt = "";
        private Vector2 _scrollPosition;
        private readonly List<Entity> _activeRequests = new();
        private bool _shouldFocusInput;

        private Texture2D _backgroundTex;
        private Texture2D _inputBgTex;
        private Texture2D _userBubbleTex;
        private Texture2D _aiBubbleTex;

        private GUIStyle _userStyle;
        private GUIStyle _aiStyle;
        private GUIStyle _inputStyle;
        private GUIStyle _headerStyle;
        private GUIStyle _labelStyle;

        private readonly Color _bgColor = new(0.20f, 0.20f, 0.20f, 1f);
        private readonly Color _inputBgColor = new(0.25f, 0.25f, 0.26f, 1f);
        private readonly Color _userBubbleColor = new(0.20f, 0.20f, 0.20f, 0f);
        private readonly Color _aiBubbleColor = new(0.27f, 0.27f, 0.28f, 1f);
        private readonly Color _textColor = new(0.93f, 0.93f, 0.93f, 1f);
        private readonly Color _systemLabelColor = new(0.56f, 0.56f, 0.58f, 1f);

        private void Start()
        {
            if (World.DefaultGameObjectInjectionWorld != null)
            {
                _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            }
            else
            {
                Debug.LogError("[LLMTester] No default world found.");
            }

            _backgroundTex = CreateTexture(_bgColor);
            _inputBgTex = CreateTexture(_inputBgColor);
            _userBubbleTex = CreateTexture(_userBubbleColor);
            _aiBubbleTex = CreateTexture(_aiBubbleColor);
        }

        private void InitStyles()
        {
            if (_userStyle != null)
            {
                return;
            }

            _userStyle = CreateBubbleStyle(_userBubbleTex, TextAnchor.UpperRight, new RectOffset(80, 20, 10, 10));
            _userStyle.normal.textColor = _textColor;

            _aiStyle = CreateBubbleStyle(_aiBubbleTex, TextAnchor.UpperLeft, new RectOffset(20, 80, 10, 10));
            _aiStyle.normal.textColor = _textColor;

            _inputStyle = new GUIStyle(GUI.skin.textArea)
            {
                fontSize = 15,
                wordWrap = true,
                padding = new RectOffset(12, 12, 12, 12),
                normal = { textColor = _textColor, background = _inputBgTex },
                active = { textColor = _textColor, background = _inputBgTex },
                focused = { textColor = _textColor, background = _inputBgTex },
                hover = { textColor = _textColor, background = _inputBgTex },
                border = new RectOffset(0, 0, 0, 0),
            };

            _headerStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 16,
                fontStyle = FontStyle.Normal,
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = _textColor },
            };

            _labelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                alignment = TextAnchor.MiddleLeft,
                normal = { textColor = _systemLabelColor },
                fontStyle = FontStyle.Bold,
            };
        }

        private GUIStyle CreateBubbleStyle(Texture2D bg, TextAnchor align, RectOffset margin)
        {
            return new GUIStyle(GUI.skin.box)
            {
                normal = { background = bg, textColor = _textColor },
                alignment = align,
                padding = new RectOffset(16, 16, 16, 16),
                margin = margin,
                wordWrap = true,
                fontSize = 15,
                richText = true,
                border = new RectOffset(0, 0, 0, 0),
            };
        }

        private void OnGUI()
        {
            InitStyles();
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), _backgroundTex);

            float padding = 20f;
            float inputHeight = 60f;
            float headerHeight = 30f;
            float chatHeight = Screen.height - inputHeight - headerHeight - (padding * 3);

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.Space(10);
            GUILayout.Label("Spellwright LLM", _headerStyle, GUILayout.Height(headerHeight));

            float centerWidth = Mathf.Min(Screen.width, 800);
            float sideMargin = (Screen.width - centerWidth) / 2;

            GUILayout.BeginArea(new Rect(sideMargin, headerHeight + 10, centerWidth, chatHeight));
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            DrawRequestList();
            GUILayout.EndScrollView();
            GUILayout.EndArea();

            float inputY = Screen.height - inputHeight - padding;
            GUILayout.BeginArea(new Rect(sideMargin, inputY, centerWidth, inputHeight + padding));
            DrawInputArea(inputHeight);
            GUILayout.EndArea();

            GUILayout.EndArea();

            if (_shouldFocusInput && Event.current.type == EventType.Repaint)
            {
                GUI.FocusControl("PromptInput");
                _shouldFocusInput = false;
            }
        }

        private void DrawInputArea(float height)
        {
            Event e = Event.current;
            bool isEnterPressed = e.type == EventType.KeyDown && e.keyCode == KeyCode.Return;
            bool isFocused = GUI.GetNameOfFocusedControl() == "PromptInput";

            if (isEnterPressed && isFocused && !e.shift)
            {
                e.Use();
                SendRequest();
            }

            GUILayout.BeginHorizontal();

            GUI.SetNextControlName("PromptInput");
            _prompt = GUILayout.TextArea(_prompt, _inputStyle, GUILayout.Height(height));

            GUILayout.Space(10);

            if (GUILayout.Button("➤", GUILayout.Width(50), GUILayout.Height(height)))
            {
                SendRequest();
            }

            GUILayout.EndHorizontal();
        }

        private void SendRequest()
        {
            _shouldFocusInput = true;

            if (string.IsNullOrWhiteSpace(_prompt))
            {
                return;
            }

            Entity entity = _entityManager.CreateEntity();
            _entityManager.AddComponentObject(entity, new LLMRequestComponent { Prompt = _prompt.Trim() });
            _activeRequests.Add(entity);

            _prompt = "";
            _scrollPosition.y = float.MaxValue;
        }

        private void DrawRequestList()
        {
            for (int i = 0; i < _activeRequests.Count; i++)
            {
                Entity entity = _activeRequests[i];
                if (!_entityManager.Exists(entity))
                {
                    continue;
                }

                LLMRequestComponent request = _entityManager.GetComponentObject<LLMRequestComponent>(entity);

                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.Box($"<b>You</b>\n{request.Prompt}", _userStyle, GUILayout.MaxWidth(Screen.width * 0.6f));
                GUILayout.EndHorizontal();

                GUILayout.Space(10);

                string aiContent;
                bool isError = false;
                bool isProcessing = false;

                if (_entityManager.HasComponent<LLMResponseComponent>(entity))
                {
                    LLMResponseComponent response = _entityManager.GetComponentObject<LLMResponseComponent>(entity);
                    if (response.IsError)
                    {
                        isError = true;
                        aiContent = $"<color=#ff6b6b>Error: {response.ErrorMessage}</color>";
                    }
                    else
                    {
                        aiContent = response.Content;
                        if (!response.IsComplete)
                        {
                            isProcessing = true;
                        }
                    }
                }
                else if (_entityManager.HasComponent<LLMProcessingTag>(entity))
                {
                    aiContent = "<i>...</i>";
                    isProcessing = true;
                }
                else
                {
                    aiContent = "<i>Pending...</i>";
                }

                if (isProcessing && !isError)
                {
                    aiContent += " <b>|</b>";
                }

                GUILayout.BeginHorizontal();
                GUILayout.Box($"<b>Spellwright</b>\n{aiContent}", _aiStyle, GUILayout.MaxWidth(Screen.width * 0.8f));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.Space(20);
            }
        }

        private void ClearHistory()
        {
            if (World.DefaultGameObjectInjectionWorld == null)
            {
                return;
            }
            foreach (Entity entity in _activeRequests)
            {
                if (_entityManager.Exists(entity))
                {
                    _entityManager.DestroyEntity(entity);
                }
            }
            _activeRequests.Clear();
        }

        private void OnDestroy()
        {
            ClearHistory();
            if (_userBubbleTex)
            {
                Destroy(_userBubbleTex);
            }
            if (_aiBubbleTex)
            {
                Destroy(_aiBubbleTex);
            }
            if (_backgroundTex)
            {
                Destroy(_backgroundTex);
            }
            if (_inputBgTex)
            {
                Destroy(_inputBgTex);
            }
        }

        private Texture2D CreateTexture(Color col)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, col);
            tex.Apply();
            return tex;
        }
    }
}
