using edeastudio.Utils;
using UnityEditor;
using UnityEngine;
using static edeastudio.Utils.Util;
using Object = UnityEngine.Object;

namespace edeastudio.Tools.Editor
{

    public class TextureChannelPacker : EditorWindow
    {
        public Texture2D m_metallic, m_ambientOcclusion, m_detailMask, m_smoothness, m_maskMap, m_tex;
        private Texture2D _metallic, _ao, _detail, _smooth;

        public string m_textureName = "Untitled";

        public int m_width = 2048, m_height = 2048;
        public bool m_inverseSmoothness = false;

        Vector2 rect = new(536, 634);

        GUISkin eSkin;

        Texture2D m_Logo;
        Texture2D m_buttonIcon;

        private int _sizeSelected = 2;
        private int _typeSelected = 0;
        private bool _enableControl;

        private readonly string[] m_texturesSize = new string[] { "512x512", "1024x1024", "2048x2048", "4096x4096" };
        private readonly string[] m_textureOut = new string[] { "MASK MAP (M, AO, H, S)", "ALBEDO ALPHA (COLOR, OPACITY)" };
        private string GetPath
        {
            get
            {
                string _path = "";

                if (m_metallic != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_metallic);
                    _path = _path[.._path.IndexOf(m_metallic.name)];
                }

                if (m_ambientOcclusion != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_ambientOcclusion);
                    _path = _path[.._path.IndexOf(m_ambientOcclusion.name)];
                }

                if (m_detailMask != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_detailMask);
                    _path = _path[.._path.IndexOf(m_detailMask.name)];
                }

                if (m_smoothness != null)
                {
                    _path = AssetDatabase.GetAssetPath((Object)m_smoothness);
                    _path = _path[.._path.IndexOf(m_smoothness.name)];
                }

                return _path;
            }
        }

        private static TextureChannelPacker instance = null;

        public TextureChannelPacker()
        {
            instance = this;
        }

        [MenuItem("edeaStudio/Tools/Texture Channel Packer &#t")]

        public static void ShowWindow()
        {
            if (instance == null)
            {
                // "Get existing open window or if none, make a new one:" says documentation.
                // But if called after script reloads a second instance will be opened! => Custom singleton required.
                TextureChannelPacker window = EditorWindow.GetWindow<TextureChannelPacker>(true);
                window.titleContent = new GUIContent("Texture Channel Packer");
                instance = window;
                instance.Show();
            }
            else
            {
                instance.Focus();
            }
        }

        public void OnGUI()
        {
            m_Logo = Resources.Load("UI/Images/icon_v2") as Texture2D;
            m_buttonIcon = Resources.Load("UI/GUI/buttonicon") as Texture2D;

            Texture2D _logo = ScaleTexture(m_Logo, 48, 48);
            Texture2D _buttonIcon = ScaleTexture(m_buttonIcon, 32, 32);

            if (!eSkin)
            {
                eSkin = Resources.Load("eSkin") as GUISkin;
            }
            GUI.skin = eSkin;

            instance.minSize = rect;
            instance.maxSize = rect;

            GUIStyle labelStyle = new GUIStyle(eSkin.GetStyle("labeltex"));
            GUIStyle textStyle = new GUIStyle(eSkin.GetStyle("textfield"));
            GUIStyle intStyle = new GUIStyle(eSkin.GetStyle("intfield"));

            textStyle.fontSize = 12;

            GUILayout.BeginVertical("TEXTURE CHANNEL PACKER", "window");
            GUILayout.Label(_logo, GUILayout.MaxHeight(48));

            GUILayout.EndVertical();

            GUILayout.BeginVertical("TEXTURES", "texturebox");
            GUILayout.Space(20);


            GUILayout.BeginVertical("", "texturebox");
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Name", labelStyle);

            GUILayoutOption[] layoutOptions = new GUILayoutOption[] { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(40) };
            GUILayoutOption[] options = new GUILayoutOption[] { GUILayout.MaxWidth(400), GUILayout.Height(EditorGUIUtility.singleLineHeight) };

            GUI.SetNextControlName("inText");

            m_textureName = EditorGUILayout.TextField(m_textureName, textStyle, options);

            GUI.SetNextControlName("inButton");

            if (GUILayout.Button(_buttonIcon, "buttonicon", layoutOptions))
            {
                if (m_metallic != null)
                {
                    m_textureName = m_metallic.name + ((_typeSelected == 0) ? "_maskMap" : "_opacity");
                    Debug.Log(m_textureName);
                    GUI.FocusControl("inText");
                    GUI.FocusControl("inButton");
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(20);

            options = new GUILayoutOption[] { GUILayout.MaxWidth(50) };

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Width", labelStyle);
            GUI.SetNextControlName("width");
            m_width = EditorGUILayout.IntField(m_width, intStyle, options);
            GUILayout.Space(10);
            EditorGUILayout.LabelField("Height", labelStyle);
            GUI.SetNextControlName("height");
            m_height = EditorGUILayout.IntField(m_height, intStyle, options);
            GUILayout.Space(100);

            EditorGUI.BeginChangeCheck();

            GUIStyle _popupStyle = new GUIStyle();
            _popupStyle = EditorStyles.popup;
            _popupStyle.font = eSkin.GetStyle("texturebox").font;

            _sizeSelected = EditorGUILayout.Popup(_sizeSelected, m_texturesSize, _popupStyle);

            if (EditorGUI.EndChangeCheck())
            {
                //Debug.Log(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                int textureSize = int.Parse(m_texturesSize[_sizeSelected][..m_texturesSize[_sizeSelected].IndexOf("x")]);
                m_width = textureSize;
                m_height = textureSize;

                GUI.FocusControl("height");
                GUI.FocusControl("width");
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Separator();

            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Select Texture Output", labelStyle);
            _typeSelected = EditorGUILayout.Popup(_typeSelected, m_textureOut, _popupStyle);

            //Debug.Log(m_textureOut[_typeSelected]);

            GUILayout.EndHorizontal();


            GUILayout.EndVertical();

            GUILayout.Space(10);

            GUILayout.BeginVertical("", "ObjectField");

            GUILayoutOption[] textureFieldLayout = new GUILayoutOption[]
            {
                GUILayout.Height(150),
                GUILayout.Width(250),
                GUILayout.ExpandWidth(false),
                GUILayout.ExpandHeight(false),
            };

            #region Upper
            GUILayout.BeginHorizontal();

            Texture2D _red = new Texture2D(250, 150);
            Texture2D _green = new Texture2D(250, 150);
            Texture2D _blue = new Texture2D(250, 150);
            Texture2D _alpha = new Texture2D(250, 150);

            _red.SimpleTexture(BackColor.Red, 250, 150);
            _green.SimpleTexture(BackColor.Green, 250, 150);
            _blue.SimpleTexture(BackColor.Blue, 250, 150);
            _alpha.SimpleTexture(BackColor.Alpha, 250, 150);
            labelStyle.fontSize = 12;
            GUILayout.BeginVertical(_red, "TextureGUI", textureFieldLayout);
            //GUI.skin = null;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("RED", labelStyle, GUILayout.Width(30));
            m_metallic = ShowTextureGUI("", m_metallic);
            //GUI.skin = eSkin;

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical(_green, "TextureGUI", textureFieldLayout);
            //GUI.skin = null;
            GUILayout.BeginHorizontal();

            _enableControl = (_typeSelected == 0);
            GUI.enabled = _enableControl;

            EditorGUILayout.LabelField("GREEN", labelStyle);
            m_ambientOcclusion = ShowTextureGUI("", m_ambientOcclusion);
            //GUI.skin = eSkin;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            #endregion

            GUILayout.Space(5);

            #region Lower
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(_blue, "TextureGUI", textureFieldLayout);
            //GUI.skin = null;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("BLUE", labelStyle);

            m_detailMask = ShowTextureGUI("", m_detailMask);

            //GUI.skin = eSkin;
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(5);

            GUILayout.BeginVertical(_alpha, "TextureGUI", textureFieldLayout);
            //GUI.skin = null;
            GUILayout.BeginHorizontal();
            GUI.enabled = true;
            EditorGUILayout.LabelField("ALPHA", labelStyle);
            m_smoothness = ShowTextureGUI("", m_smoothness);
            GUILayout.EndHorizontal();
            /* if (m_smoothness)
             {
                 EditorGUI.PrefixLabel(new Rect(25, 45, 100, 15), 0, new GUIContent("Preview:"));
                 EditorGUI.DrawPreviewTexture(new Rect(25, 60, 100, 100), m_smoothness);
             }*/
            //GUI.skin = eSkin;
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Invert", labelStyle);
            m_inverseSmoothness = EditorGUILayout.Toggle(m_inverseSmoothness, "toggle");
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
            #endregion

            GUILayout.EndVertical();

            GUILayout.EndVertical();


            EditorGUILayout.Separator();

            if (GUILayout.Button("Pack Textures"))
            {
                PackTextures();
            }

            if (GUILayout.Button("Clear"))
            {
                Clear();
            }

            // Debug.Log($"Width : {position.width} - Height : {position.height}");

        }


        private void Clear()
        {
            m_metallic = null;
            m_ambientOcclusion = null;
            m_detailMask = null;
            m_smoothness = null;
            m_maskMap = null;
            m_textureName = "Untitled";
            m_width = 2048;
            m_height = 2048;
            m_inverseSmoothness = false;
            _sizeSelected = 2;
            _typeSelected = 0;
            GUI.FocusControl("inText");
        }
        private void PackTextures()
        {
            if (m_metallic == null && m_ambientOcclusion == null && m_detailMask == null && m_smoothness == null) return;
            AdjustTexturesSize(m_width, m_height);

            m_maskMap = new Texture2D(m_width, m_height);
            m_maskMap.SetPixels(ColorsArray());

            byte[] m_tex = m_maskMap.EncodeToPNG();

            string path = EditorUtility.SaveFilePanel("Save Icon image file", "Assets/", m_textureName, "png");

            if (string.IsNullOrEmpty(path)) return;

            path = FileUtil.GetPhysicalPath(path);

            System.IO.File.WriteAllBytes(path, m_tex);
            Debug.Log(m_tex.Length / 1024 + "Kb was saved as: " + path);
#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
            /*
            FileStream _stream = new FileStream(GetPath + m_textureName + ".png", FileMode.OpenOrCreate, FileAccess.ReadWrite);

            BinaryWriter _writer = new(_stream);

            for (int i = 0; i < m_tex.Length; i++)
            {
                _writer.Write(m_tex[i]);
            }

            _stream.Close();
            _writer.Close();

            AssetDatabase.ImportAsset(GetPath + m_textureName + ".png", ImportAssetOptions.ForceUpdate);
            AssetDatabase.Refresh();*/
        }

        private void AdjustTexturesSize(int t_width, int t_height)
        {

            if (m_metallic != null)
            {
                _metallic = GPUTextureScaler.Scaled(m_metallic, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_ambientOcclusion != null)
            {
                _ao = GPUTextureScaler.Scaled(m_ambientOcclusion, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_detailMask != null)
            {
                _detail = GPUTextureScaler.Scaled(m_detailMask, t_width, t_height, FilterMode.Bilinear);
            }
            if (m_smoothness != null)
            {
                _smooth = GPUTextureScaler.Scaled(m_smoothness, t_width, t_height, FilterMode.Bilinear);
            }
        }

        private Color[] ColorsArray()
        {
            Color[] m_colors = new Color[m_width * m_height];

            for (int i = 0; i < m_colors.Length; i++)
            {
                m_colors[i] = new Color();

                if (_metallic != null)
                {
                    m_colors[i].r = _metallic.GetPixel(i % m_width, i / m_width).r;
                }
                else
                {
                    m_colors[i].r = 1;
                }

                if (_ao != null)
                {
                    m_colors[i].g = _ao.GetPixel(i % m_width, i / m_width).g;
                }
                else
                {
                    if (_typeSelected == 1) m_colors[i].g = _metallic.GetPixel(i % m_width, i / m_width).g;
                    else m_colors[i].g = 1;
                }

                if (_detail != null)
                {
                    m_colors[i].b = _detail.GetPixel(i % m_width, i / m_width).b;
                }
                else
                {
                    if (_typeSelected == 1) m_colors[i].b = _metallic.GetPixel(i % m_width, i / m_width).b;
                    else m_colors[i].b = 1;
                }

                if (_smooth != null)
                {
                    m_colors[i].a = m_inverseSmoothness ? 1 - _smooth.GetPixel(i % m_width, i / m_width).r : _smooth.GetPixel(i % m_width, i / m_width).r;
                }
                else
                {
                    m_colors[i].a = 1;
                }
            }

            return m_colors;

        }
        /*      public static class CustomEditorGUILayout
                {
                    public static void ObjectField<T>(string label, T obj, bool allowSceneReferences) where T : UnityEngine.Object
                    {
                        obj = (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneReferences);
                    }
                }*/
        public Texture2D ShowTextureGUI(string fieldName, Texture2D texture)
        {
            GUILayoutOption[] options = { GUILayout.Height(EditorGUIUtility.singleLineHeight), GUILayout.Width(150), };
            return (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, options);
        }
    }

}