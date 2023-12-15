using System;
using System.Reflection;
using CookApps.LocalData;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CookApps
{
    public class LocalDataSample : MonoBehaviour
    {
        //--------------------------------------------------------------------------------//
        //-----------------------------------FIELD----------------------------------------//
        //--------------------------------------------------------------------------------//
        //------------------- Inspector ------------------//

        //------------------- public ------------------//

        //------------------- protected ------------------//

        //------------------- private ------------------//
        private string _result = "";
        private GUIStyle _headerStyle;
        private SampleData _sampleData;
        private string _inputInt;
        private string _inputString;

        private CookAppsLocalData _localData;
        private CookAppsLocalData _localData2;
        private static readonly string FILE_NAME = "local_data_sample.dat";

        //--------------------------------------------------------------------------------//
        //------------------------------------PROPERTY------------------------------------//
        //--------------------------------------------------------------------------------//

        //--------------------------------------------------------------------------------//
        //------------------------------------METHOD--------------------------------------//
        //--------------------------------------------------------------------------------//
        //───────────────────────────────────────────────────────────────────────────────────────
        private void Awake()
        {
            _headerStyle = new GUIStyle();
            _headerStyle.fontSize = 30;
            _headerStyle.fontStyle = FontStyle.Bold;

            _localData = new CookAppsLocalData(GetKey());
            if (_localData.TryLoad(FILE_NAME, out _sampleData) == false)
            {
                _sampleData = new SampleData();
            }

            SetSampleData();
        }

        public static string GetKey()
        {
            //https://cookapps.atlassian.net/wiki/spaces/TST/pages/25222972384/string 를 참고하세요.
            // key = "cookapps1357@$^*"
            var key =
                "\u4ACA\u4AB0\u4AB0\u4AAE\u4AC7\uCAAF\uCAAF\u4AB2\u4ACF\u4AD2\u4AD9\u4AD4\uCAD7\uCAF1\uCAC8\uCACE";

            for (int oyXIM = 0, alRfs = 0; oyXIM < 16; oyXIM++)
            {
                alRfs = key[oyXIM];
                alRfs -= 0x6C1E;
                alRfs ^= 0x56DC;
                alRfs++;
                alRfs ^= 0x22D2;
                alRfs += 0x5B27;
                alRfs = ((alRfs << 13) | ((alRfs & 0xFFFF) >> 3)) & 0xFFFF;
                alRfs++;
                alRfs = ((alRfs << 4) | ((alRfs & 0xFFFF) >> 12)) & 0xFFFF;
                alRfs = ~alRfs;
                alRfs -= 0xF3F8;
                key = key.Substring(0, oyXIM) + (char) (alRfs & 0xFFFF) + key.Substring(oyXIM + 1);
            }

            return key;
        }

#if UNITY_EDITOR && TECH_DEBUG
        [MenuItem("CookApps/Delete PlayerPrefs")]
        private static void DeletePlayerPrefs()
        {
            PlayerPrefs.DeleteKey(FILE_NAME);
        }
#endif

        private void OnGUI()
        {
            GUILayout.Label($"SampleData----------------------------------------");
            GUILayout.BeginHorizontal();
            GUILayout.Label($"publicIntFieldValue : ", GUILayout.Height(60));
            _inputInt = GUILayout.TextField(_inputInt, GUILayout.Height(60));
            _sampleData.publicIntFieldValue = Convert.ToInt32(_inputInt);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label($"publicStringFieldValue : ", GUILayout.Height(60));
            _inputString = GUILayout.TextField(_inputString, GUILayout.Height(60));
            _sampleData.publicStringFieldValue = _inputString;
            GUILayout.EndHorizontal();
            GUILayout.Label("--------------------------------------------------------------------------------");

            //--------------------------------------------------Save
            if (GUILayout.Button("Save", GUILayout.Width(300), GUILayout.Height(100)))
            {
                CookAppsLocalData.EnumSaveResult enumSaveResult = _localData.Save(_sampleData, FILE_NAME);

                switch (enumSaveResult)
                {
                    case CookAppsLocalData.EnumSaveResult.SUCCESS:
                        _result = "저장 완료!";
                        Debug.Log(_result);
                        break;

                    case CookAppsLocalData.EnumSaveResult.FAIL_UNKNOWN:
                    case CookAppsLocalData.EnumSaveResult.FAIL_DISK_FULL:
                        _result = $"저장에 실패했습니다. 이유 : {enumSaveResult}. 디바이스의 디스크 공간을 확보하세요.";
                        Debug.LogError(_result);
                        break;
                }
            }

            //--------------------------------------------------Load
            if (GUILayout.Button("Load", GUILayout.Width(300), GUILayout.Height(100)))
            {
                if (_localData.TryLoad<SampleData>(FILE_NAME, out SampleData sampleData))
                {
                    _result = "데이터가 null입니다.";
                    Debug.LogError(_result);
                }
                else
                {
                    _result =
                        $"로드완료 - publicIntFieldValue : {sampleData.publicIntFieldValue}, publicStringFieldValue : {sampleData.publicStringFieldValue}";
                    Debug.Log(_result);

                    SetSampleData();
                }
            }

            //--------------------------------------------------Load
            if (GUILayout.Button("Delete", GUILayout.Width(300), GUILayout.Height(100)))
            {
                _localData.Delete(FILE_NAME);
                Debug.Log($"{FILE_NAME}을 제거했습니다.");
            }

            GUILayout.Label($"결과 : {_result}", _headerStyle);
        }

        private void SetSampleData()
        {
            _inputInt = _sampleData.publicIntFieldValue.ToString();
            _inputString = _sampleData.publicStringFieldValue;
        }
    }
}
