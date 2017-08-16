﻿
using KSP.IO;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using File = KSP.IO.File;

namespace KaptainsLogNamespace
{
    public class ImageViewer
    {
        private Texture2D _image;
        private WWW _imagetex;
        private Rect _windowRect;
        private Rect _windowRect2 = new Rect(Screen.width / 2 - 150f, Screen.height / 2 - 75f, 260f, 390f);
        bool resetSize = false;
        bool updateSize = false;
        string _imagefile;
        public bool visible = false;
        bool everVisible = false;

        ~ImageViewer()
        {
            if (_image != null)
                Object.Destroy(_image);
        }

        public ImageViewer(string fname)
        {
            LoadImage(fname);
            
        }

        public void LoadImage(string fname)
        {
            if (System.IO.File.Exists(fname))
            {
                _imagefile = "file://" + fname;
                ImageOrig();
                visible = true;                
            }
            else
                Log.Info("File does not exist");
        }

        private void ImageOrig()
        {
            _imagetex = new WWW(_imagefile);
            _image = _imagetex.texture;
            _imagetex.Dispose();

          

            if (_image.width > Screen.width/2 || _image.height > Screen.height/2)
            {

                float finalRatio = Mathf.Min((float)(Screen.width/2) / (float)_image.width, (float)(Screen.height/2) / (float)_image.height);

                float finalWidth = (float)_image.width * finalRatio;
                float finalHeight = (float)_image.height * finalRatio;

                TextureScale.Bilinear(_image, (int)finalWidth, (int)finalHeight);

            }
            updateSize = true;
        }
        private void ImageZm()
        {
            TextureScale.Bilinear(_image, _image.width - ((_image.width * 10) / 100), _image.height - ((_image.height * 10) / 100));
            updateSize = true;
        }
        private void ImageZp()
        {
            TextureScale.Bilinear(_image, _image.width + ((_image.width * 10) / 100), _image.height + ((_image.height * 10) / 100));
            updateSize = true;
            
        }

        public void OnGUI()
        {
            // Saves the current Gui.skin for later restore
            GUISkin _defGuiSkin = GUI.skin;
            
            {
                if (resetSize)
                {
                    ImageOrig();
                    resetSize = false;
                }
                //GUI.skin = _useKSPskin ? HighLogic.Skin : _defGuiSkin;
                //_windowRect = GUI.Window(GUIUtility.GetControlID(0, FocusType.Passive), _windowRect, IvWindow,
                //    "Image viewer");
                if (updateSize)
                {
                    float x, y;

                    x = _windowRect.x + _windowRect.width / 2;
                    y = _windowRect.y + _windowRect.height / 2;
                    _windowRect = new Rect((Screen.width - _image.width) / 2f, (Screen.height - _image.height) / 2f, _image.width, _image.height + 30);

                    if (!everVisible)
                    {
                        everVisible = true;

                        _windowRect.x = (Screen.width - _image.width) / 2f;
                        _windowRect.y = (Screen.height - _image.height) / 2f;
                    }
                    else
                    {
                        _windowRect.x = x - _windowRect.width / 2;
                        _windowRect.y = y - _windowRect.height / 2;
                    }
                    updateSize = false;
                }
                _windowRect = GUILayout.Window(GUIUtility.GetControlID(0, FocusType.Passive), _windowRect, IvWindow,
                        "Image viewer");
            }

            //Restore the skin
            GUI.skin = _defGuiSkin;

        }
        public void IvWindow(int windowID)
        {
            if (_image != null)
            {
                //_windowRect = new Rect(_windowRect.xMin, _windowRect.yMin, _image.width, _image.height + 20f);
                //GUI.DrawTexture(new Rect(0f, 20f, _image.width, _image.height), _image, ScaleMode.ScaleToFit, true, 0f);
                GUILayout.BeginHorizontal();
                GUILayout.Box(_image);
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("-10% size"))
                {
                    ImageZm();
                }
                if (GUILayout.Button("Original size"))
                {
                    ImageOrig();
                }
                if (GUILayout.Button("+10% size"))
                {
                    ImageZp();
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Close"))
                    Toggle();
                GUILayout.EndHorizontal();
                //_windowRect = new Rect(_windowRect.xMin, _windowRect.yMin, _image.width, _image.height + 20f);
                //GUI.DrawTexture(new Rect(0f, 20f, _image.width, _image.height), _image, ScaleMode.ScaleToFit, true, 0f);
            }
            else
            {
                _windowRect = new Rect(Screen.width / 2f, Screen.height / 2f, 100f, 100f);
            }
            if (GUI.Button(new Rect(2f, 2f, 13f, 13f), "X"))
            {
                Toggle();
                
            }
            GUI.DragWindow();
        }
        void Toggle()
        {
            KaptainsLog.Instance.displayScreenshot = false;
            Object.Destroy(_image);
            _image = null;
            visible = false;
        }
    }
}
