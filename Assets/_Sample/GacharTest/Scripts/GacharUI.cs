using UnityEngine;
using TMPro;

namespace MySampleEx
{
    /// <summary>
    /// 가차 뽑기 진행 상태
    /// </summary>
    public enum GacharState
    {
        Ready,          //시작 버튼을 기다리는 상태
        Scroll01,       //스타트 버튼을 누르면 애니01 플레이, 아이템은 무작위, 스톱 버튼을 기다린다
        Scroll02,       //스톱 버튼을 누르면 가차 아이템이 뽑힌 상태, 애니02 플레이, 공회전(3번)
        Scroll03,       //애니02 플레이, 일정 시간이 지나면 정답을 보여준다, 랜덤하게 1~3 스크롤을 해준다
        Result          //정답을 보여준다
    }

    /// <summary>
    /// 가차 데이터를 가져와서 뽑기를 진행
    /// </summary>
    public class GacharUI : MonoBehaviour
    {
        #region Variables
        public GacharItems gacharList;
        public string dataPath;

        //UI
        public TextMeshProUGUI realName;
        public TextMeshProUGUI fakeName;

        public TextMeshProUGUI sentence;

        public GameObject startButton;
        public GameObject stopButton;
        public GameObject nextButton;

        private Animator animator;
        private GacharState gacharState;

        //뽑기
        private int nowIndex = 0;
        private int dummyNumber = 3;    //scroll2 3회전 공회전
        private int fakeNumber = 0;     //랜덤 스크롤 횟수
        private int scrollCount = 0;

        public int gacharIndex = 0;    //가차에 뽑횐 아이템 인덱스

        [SerializeField] private string nameText = "누구?";
        #endregion

        private void Start()
        {
            //참조
            animator = GetComponent<Animator>();
            gacharList = GacharManager.GetGacharData().LoadData(dataPath);
        }

        //가차 초기화
        public void SetGachar(bool isReady)
        {
            if (isReady)
            {
                realName.text = nameText;
                fakeName.text = nameText;
                sentence.text = "시작 버튼을 누르세요";

                startButton.SetActive(true);
            }
            else
            {
                realName.text = "";
                fakeName.text = "";
                sentence.text = "";

                startButton.SetActive(false);
            }
        }

        public void StartGachar()
        {
            animator.Play("NameScroll01");

            startButton.SetActive(false);
            stopButton.SetActive(true);

            sentence.text = "스톱 버튼을 누르세요";

            //뽑기 데이터 초기
            scrollCount = 0;
            nowIndex = 1;
            realName.text = gacharList.gacharItems[nowIndex].name;
            fakeName.text = gacharList.gacharItems[nowIndex-1].name;
        }

        public void StopGachar()
        {
            //뽑기
            gacharIndex = GetGacharItem();
            gacharList.gacharItems[gacharIndex].rate = 0;

            fakeNumber = Random.Range(1, 4);
            nowIndex = gacharIndex - (dummyNumber + fakeNumber) + 1;
            GetGacharItem();

            animator.Play("NameScroll02");
            scrollCount = 0;
            stopButton.SetActive(false);
            sentence.text = "";
        }

        private int GetGacharItem()
        {
            int result = 0;

            //랜덤 범위 총합 구하기
            int total = 0;
            for (int i = 0; i < gacharList.gacharItems.Count; i++)
            {
                total += gacharList.gacharItems[i].rate;
            }

            int randNumber = Random.Range(0, total);
            int subTotal = 0;

            for (int i = 0; i < gacharList.gacharItems.Count; i++)
            {
                if (gacharList.gacharItems[i].rate == 0)
                    continue;

                subTotal += gacharList.gacharItems[i].rate;

                if (randNumber < subTotal)
                {
                    result = i; 
                    break;
                }
            }

            return result;
        }

        private void GotoScroll03()
        {
            animator.Play("NameScroll03");
            scrollCount = 0;
        }

        private void GotoResult()
        {
            animator.Play("Empty");
            nextButton.SetActive(true);
            sentence.text = gacharList.gacharItems[gacharIndex].name + "뽑혔습니다";
        }

        private void SetGacharName()
        {
            if (nowIndex < 0)
                nowIndex += gacharList.gacharItems.Count;
            if (nowIndex >= gacharList.gacharItems.Count)
                nowIndex -= gacharList.gacharItems.Count;

            if(nowIndex == 0)
            {
                realName.text = gacharList.gacharItems[nowIndex].name;
                fakeName.text = gacharList.gacharItems[gacharList.gacharItems.Count - 1].name;
            }
            else
            {
                realName.text = gacharList.gacharItems[nowIndex].name;
                fakeName.text = gacharList.gacharItems[nowIndex - 1].name;
            }
        }

        private void PlayScroll01()
        {
            nowIndex++;
            SetGacharName();
        }

        private void PlayScroll02()
        {
            scrollCount++;

            nowIndex++;
            SetGacharName();

            if(scrollCount >= dummyNumber)
            {
                GotoScroll03();
            }
        }

        private void PlayScroll03()
        {
            scrollCount++;

            nowIndex++;
            SetGacharName();

            if (scrollCount >= fakeNumber)
            {
                GotoResult();
            }
        }


    }
}
