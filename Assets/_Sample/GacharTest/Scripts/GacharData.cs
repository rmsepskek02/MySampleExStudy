using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace MySampleEx
{
    /// <summary>
    /// 가차 아이템 데이터를 관리하는 클래스
    /// </summary>
    public class GacharData : ScriptableObject
    {
        #region Variables
        public GacharItems gacharNameList;
        public GacharItems gacharItemList;
        #endregion

        //생성자
        public GacharData() { }

        //데이터 읽기
        public GacharItems LoadData(string dataPath)
        {
            TextAsset asset = (TextAsset)ResourcesManager.Load(dataPath);
            if (asset == null || asset.text == null)
            {
                return null;
            }

            GacharItems itemList;

            using (XmlTextReader reader = new XmlTextReader(new StringReader(asset.text)))
            {
                var xs = new XmlSerializer(typeof(GacharItems));
                itemList = (GacharItems)xs.Deserialize(reader);
            }

            return itemList;
        }
    }
}
