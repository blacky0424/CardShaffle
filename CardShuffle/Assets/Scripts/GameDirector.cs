using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//個人製作
//正解のカードが1枚だけあり、シャッフルしたカードの中から正解のカードをあてるミニゲーム
public class GameDirector : MonoBehaviour {
    //カードのイラスト
    public Sprite backSprite;
    public Sprite faceSprite1;
    public Sprite faceSprite2;
    //背景のイラスト
    public Sprite background1Sprite;
    public Sprite background2Sprite;
    //背景のゲームオブジェクト
    private GameObject background;
    //カード
　  public List<GameObject> cardBox = new List<GameObject>();
    private List<GameObject> useCardBox;
    private GameObject card;
    private GameObject correctCard;
    private GameObject answerCard;
    private GameObject findCard;
    //ボタン
    private GameObject buttonBox;
    //位置
    private Vector3 firstPos;
    private Vector3[] cardPos;
    //テキスト
    private GameObject levelText;
    private GameObject announceText;
    private GameObject findCardText;
    //使うカードの枚数
    private int cardNum;
    //カードの移動の速さ
    private float speed ;
    //カードが移動し始めてからの時間
    private float moveTime;
    //カードの移動時間
    private float spanTime;
    //シャッフルした回数
    private int shaffleCount;
    //シャッフルする回数
    private int shaffleFinalCount;
    //乱数
    private int random1;
    private int random2;
    //ゲームシーンのスタート
    private bool start;
    //最初のカード確認スイッチ
    private bool check;
    //シャッフルスイッチ
    private bool shaffleSwitch;
    //カードの移動スイッチ
    private bool moveSwitch;
    //回答スイッチ
    private bool answerSwitch;
    //選択シーンへ戻るスイッチ
    private bool levelSceneBack;
    //効果音
    public AudioClip tap;
    private AudioSource audio;
    void Start()
    {
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 0.5f);
        //ゲームオブジェクトを見つける
        this.background = GameObject.Find("background");
        this.levelText = GameObject.Find("level");
        this.announceText = GameObject.Find("announce");
        this.findCardText = GameObject.Find("FindCardText");
        this.card = GameObject.Find("cardBox");
        this.findCard = GameObject.Find("FindCard");
        this.findCard.active = false;
        this.buttonBox = GameObject.Find("buttonBox");
        this.levelText.active = false;
        this.findCardText.GetComponent<Text>();
        this.findCardText.active = false;
        audio = GetComponent<AudioSource>();
        //cardの山札の位置を保存
        this.firstPos = this.cardBox[0].transform.position;
        this.card.active = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (this.start)
        {
            this.start = false;
            this.levelText.active = true;
            this.card.active = true;
            this.findCard.active = true;
            this.findCardText.active = true;
            //announceTextの変更
            this.announceText.GetComponent<Text>().text = "Ready?";
            //背景変更
            this.background.transform.localScale = new Vector3(0.6f, 0.4f, 1.0f);
            this.background.GetComponent<SpriteRenderer>().sprite = this.background2Sprite;
            //カードの移動時間の初期化
            this.moveTime = 0;
            //シャッフルした回数の初期化
            this.shaffleCount = 0;
            //使うcardをいれる
            this.useCardBox = new List<GameObject>(this.cardBox);
            for (var i = 0; i < this.cardBox.Count - this.cardNum; i++)
            {
                this.useCardBox.RemoveAt(this.useCardBox.Count - 1);
            }
            //cardPosの要素数決定
            this.cardPos = new Vector3[this.useCardBox.Count];
            //カードの枚数により配られるカードの定位置が変わる
            switch (this.cardNum)
            {
                case 2:
                    this.cardPos[0] = new Vector3(-2, 0, 0);
                    this.cardPos[1] = new Vector3(2, 0, 0);
                    break;
                case 3:
                    this.cardPos[0] = new Vector3(-3, 0, 0);
                    this.cardPos[1] = new Vector3(0, 0, 0);
                    this.cardPos[2] = new Vector3(3, 0, 0);
                    break;
                case 4:
                    this.cardPos[0] = new Vector3(-2, 2, 0);
                    this.cardPos[1] = new Vector3(2, 2, 0);
                    this.cardPos[2] = new Vector3(-2, -2, 0);
                    this.cardPos[3] = new Vector3(2, -2, 0);
                    break;
                case 5:
                    this.cardPos[0] = new Vector3(-3, 2, 0);
                    this.cardPos[1] = new Vector3(0, 2, 0);
                    this.cardPos[2] = new Vector3(3, 2, 0);
                    this.cardPos[3] = new Vector3(-2, -2, 0);
                    this.cardPos[4] = new Vector3(2, -2, 0);
                    break;
                case 6:
                    this.cardPos[0] = new Vector3(-3, 2, 0);
                    this.cardPos[1] = new Vector3(0, 2, 0);
                    this.cardPos[2] = new Vector3(3, 2, 0);
                    this.cardPos[3] = new Vector3(-3, -2, 0);
                    this.cardPos[4] = new Vector3(0, -2, 0);
                    this.cardPos[5] = new Vector3(3, -2, 0);
                    break;
                case 7:
                    this.cardPos[0] = new Vector3(-3, 2, 0);
                    this.cardPos[1] = new Vector3(0, 2, 0);
                    this.cardPos[2] = new Vector3(3, 2, 0);
                    this.cardPos[3] = new Vector3(-3, -2, 0);
                    this.cardPos[4] = new Vector3(0, -2, 0);
                    this.cardPos[5] = new Vector3(3, -2, 0);
                    this.cardPos[6] = new Vector3(-6, 0, 0);
                    break;
            }
            //正解カードをランダムに決める
            int correct = Random.Range(0, this.useCardBox.Count);
            //正解カードの格納
            this.correctCard = this.useCardBox[correct];
            for (int i = 0; i < this.useCardBox.Count; i++)
            {
                //カードのサーブ
                iTween.MoveTo(this.useCardBox[i], iTween.Hash("x", this.cardPos[i].x, "y", this.cardPos[i].y, "time", 2.0f, "delay", 2.0f));
                this.scaleChange(this.useCardBox[i], 1.0f, 1.0f, 4.0f, 5.0f, true);
            }
            //使わないカードをなくす
            for (int i = this.cardNum; i < this.cardBox.Count; i++)
            {
                iTween.MoveTo(this.cardBox[i], iTween.Hash("x", 0, "y", 7.5f, "time", 2.0f, "delay", 3.0f));
            }
            //Ready?と表示
            Invoke("ready", 6.0f);
        }
        //カードの確認
        if (this.check)
        {
            this.announceText.active = true;
            //確認したらタッチしてカードの反転
            if (Input.GetMouseButtonDown(0))
            {
                this.check = false;
                for (var i = 0; i < this.useCardBox.Count; i++)
                {
                    this.announceText.GetComponent<Text>().text = "";
                    this.scaleChange(this.useCardBox[i], 1.0f, 1.0f, 1.0f, 2.0f, false);
                }
                //GO!と表示して、シャッフルスタート
                Invoke("go", 3.0f);
            }
        }
        //1度目のカードのシャッフル
        if (this.shaffleSwitch)
        {
            this.cardShaffle();
            this.shaffleCount++;
        }
        //カードの移動
        if (this.moveSwitch)
        {
            this.moveTime += Time.deltaTime;
            for (var i = 0; i < this.useCardBox.Count; i++)
            {
                iTween.MoveTo(this.useCardBox[i], iTween.Hash("x", this.cardPos[i].x, "y", this.cardPos[i].y, "time", this.speed, "delay", 0));
            }
        }
        //2度目以降のカードのシャッフル
        if(this.shaffleCount < this.shaffleFinalCount && this.moveTime > this.spanTime)
        {
            this.shaffleSwitch = true;
            if(this.shaffleCount != this.shaffleFinalCount)
            {
                this.moveTime = 0;
            }
        }
        //最終シャッフルが終わったら回答に
        if(this.shaffleCount == this.shaffleFinalCount && this.moveTime > this.spanTime * 1.2)
        {
            this.announceText.GetComponent<Text>().text = "What's your Answer?";
            this.announceText.active = true;
            this.moveSwitch = false;
            this.moveTime = 0;
            this.answerSwitch = true;
        }
        //タッチしたカード反転する
        if (this.answerSwitch)
        {
            if (Input.GetMouseButtonDown(0))
            {
                //タッチした位置からRayを飛ばす
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //タッチをした位置にオブジェクトがあるかどうかを判定
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit)
                {
                    //タッチしたカードを格納
                    this.answerCard = hit.collider.gameObject;
                    //タッチしたオブジェクトのcolliderを四角く切り取る
                    Bounds rect = hit.collider.bounds;
                    //切り取った部分とタップした位置が重なっていたら
                    if (rect.Contains(worldPoint))
                    {
                        //Rayを飛ばしてあたったオブジェクトがcardだったら
                        if (hit.collider.gameObject.tag == "card")
                        {
                            this.announceText.active = false;
                            this.answerSwitch = false;
                            //選んだカードを反転
                            this.resultScaleChange(this.answerCard, 1.0f, 1.0f, 0, 1.0f);
                            //遅れてそのほかのカードを反転
                            for (int i = 0; i < this.useCardBox.Count; i++)
                             {
                                 if (this.useCardBox[i] != this.answerCard)
                                 {
                                     this.scaleChange(this.useCardBox[i], 1.0f, 1.0f, 2.0f, 3.0f, true);
                                 }
                             }
                            Invoke("result", 4.5f);
                        }
                    }
                }
            }
        }
        if (this.levelSceneBack)
        {
            if (Input.GetMouseButtonDown(0))
            {
                this.levelSceneBack = false;
                //フェードイン
                CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.0f);
                //セレクトシーンに変更 初期化
                this.background.transform.localScale = new Vector3(4.0f, 3.0f, 1.0f);
                this.background.GetComponent<SpriteRenderer>().sprite = this.background1Sprite;
                this.levelText.active = false;
                this.card.active = false;
                this.buttonBox.active = true;
                this.findCard.active = false;
                this.findCardText.active = false;
                this.announceText.GetComponent<Text>().text = "どのレベルで遊びますか?";
                for(int i = 0; i < this.cardBox.Count; i++)
                {
                    this.cardBox[i].transform.position = firstPos;
                    this.cardBox[i].GetComponent<SpriteRenderer>().sprite = this.backSprite;
                }
            }
        }
    }

    private void scaleChange(GameObject card, float time1, float time2, float delay1, float delay2, bool face_Back)
    {
        //カードの反転
        iTween.ScaleTo(card, iTween.Hash("x", 0, "time", time1, "delay", delay1));
        //spriteの入れ替え
        if (face_Back)
        {
            Invoke("spriteFaceChange", delay2);
        }
        else
        {
            Invoke("spriteBackChange", delay2);
        }
        iTween.ScaleTo(card, iTween.Hash("x", 1, "time", time2, "delay", delay2));
    }

    private void resultScaleChange(GameObject card, float time1, float time2, float delay1, float delay2)
    {
        //カードの反転
        iTween.ScaleTo(card, iTween.Hash("x", 0, "time", time1, "delay", delay1));
        Invoke("resultChangeSprite", delay2);
        iTween.ScaleTo(card, iTween.Hash("x", 1, "time", time2, "delay", delay2));
    }

    private void resultChangeSprite()
    {
        if(this.answerCard == this.correctCard)
        {
            this.answerCard.GetComponent<SpriteRenderer>().sprite = this.faceSprite2;
        }
        else
        {
            this.answerCard.GetComponent<SpriteRenderer>().sprite = this.faceSprite1;
        }
    }

    private void result()
    {
        //正解か間違いかで文字が変わる
        if (this.answerCard == this.correctCard)
        {
            this.announceText.GetComponent<Text>().text = "Congratulation!";
        }
        else
        {
            this.announceText.GetComponent<Text>().text = "Don't mind";
        }
        this.announceText.active = true;
        this.levelSceneBack = true;
    }

    private void spriteFaceChange()
    {
        for (var i = 0; i < this.useCardBox.Count; i++)
        {
            this.useCardBox[i].GetComponent<SpriteRenderer>().sprite = this.faceSprite1;
        }
        this.correctCard.GetComponent<SpriteRenderer>().sprite = this.faceSprite2;
    }

    private void ready()
    {
        this.announceText.active = true;
        this.check = true;
    }
    private void go()
    {
        this.announceText.GetComponent<Text>().text = "GO!";
        Invoke("gameStart", 2.0f);
    }

    private void gameStart()
    {
        this.announceText.active = false;
        this.shaffleSwitch = true;
    }

    private void spriteBackChange()
    {
        for (var i = 0; i < this.useCardBox.Count; i++)
        {
            this.useCardBox[i].GetComponent<SpriteRenderer>().sprite = this.backSprite;
        }
    }

    private void cardShaffle()
    {
        this.shaffleSwitch = false;
        this.moveSwitch = false;
        bool ok = false;
        List<int> numBox = new List<int>();
        for (int i = 0; i < this.cardNum; i++)
        {
            numBox.Add(i);
        }
        //かぶらない乱数になるまでつづける
        while (!ok) {
            this.random1 = Random.RandomRange(0, this.cardNum);
            this.random2 = Random.RandomRange(0, this.cardNum);
            if (this.random1 != this.random2)
            {
                GameObject change1 = this.useCardBox[numBox[this.random1]];
                GameObject change2 = this.useCardBox[numBox[this.random2]];
                this.useCardBox[numBox[this.random1]] = change2;
                this.useCardBox[numBox[this.random2]] = change1;
                //使用カードが3枚以下なら1組シャッフル
                //使用カードが4枚以上なら2組シャッフル
                //使用カードが6枚以上なら3組シャッフル
                if (this.cardNum != 3 && this.cardNum != 6)
                {
                    for (int i = this.cardNum / 3; i > 0; i--)
                    {
                        if (this.random1 > this.random2)
                        {
                            numBox.RemoveAt(this.random2);
                            numBox.RemoveAt(this.random1 - 1);
                        }
                        else if(this.random1 < this.random2)
                        {
                            numBox.RemoveAt(this.random1);
                            numBox.RemoveAt(this.random2 - 1);
                        }
                        this.random1 = Random.RandomRange(0, numBox.Count);
                        this.random2 = Random.RandomRange(0, numBox.Count);
                        //フリーズを防ぐための回数制限
                        int a = 0;
                        while (!ok)
                        {
                            if (this.random1 != this.random2)
                            {
                                change1 = this.useCardBox[numBox[this.random1]];
                                change2 = this.useCardBox[numBox[this.random2]];
                                this.useCardBox[numBox[this.random1]] = change2;
                                this.useCardBox[numBox[this.random2]] = change1;
                                ok = true;
                            }
                            a++;
                            //回数超えたら強制シャッフル
                            if (a > 30)
                            {
                                change1 = this.useCardBox[numBox[0]];
                                change2 = this.useCardBox[numBox[1]];
                                this.useCardBox[numBox[1]] = change2;
                                this.useCardBox[numBox[0]] = change1;
                                break;
                            }
                        }
                    }
                }
                this.moveSwitch = true;
                ok = true;
            }
        }
    }

    private void startOn()
    {
        this.start = true;
    }

    public void level1()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 2;
        //カードの1度のシャッフル時間
        this.speed = 4.0f;
        //カードの移動時間の決定
        this.spanTime = 3.0f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 3;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level1";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level2()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 3;
        //カードの1度のシャッフル時間
        this.speed = 3.0f;
        //カードの移動時間の決定
        this.spanTime = 2.0f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 5;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level2";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level3()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 4;
        //カードの1度のシャッフル時間
        this.speed = 2.0f;
        //カードの移動時間の決定
        this.spanTime = 1.0f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 6;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level3";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level4()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 3;
        //カードの1度のシャッフル時間
        this.speed = 1.0f;
        //カードの移動時間の決定
        this.spanTime = 0.75f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 8;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level4";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level5()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 4;
        //カードの1度のシャッフル時間
        this.speed = 1.0f;
        //カードの移動時間の決定
        this.spanTime = 0.75f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 10;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level5";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level6()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 5;
        //カードの1度のシャッフル時間
        this.speed = 0.75f;
        //カードの移動時間の決定
        this.spanTime = 0.5f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 10;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level6";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level7()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 4;
        //カードの1度のシャッフル時間
        this.speed = 0.75f;
        //カードの移動時間の決定
        this.spanTime = 0.25f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 15;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level7";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level8()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 6;
        //カードの1度のシャッフル時間
        this.speed = 0.75f;
        //カードの移動時間の決定
        this.spanTime = 0.2f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 15;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level8";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
    public void level9()
    {
        audio.PlayOneShot(tap);
        //フェードイン
        CameraFade.StartAlphaFade(Color.black, true, 1.0f, 1.5f);
        //使うカードの枚数決定
        this.cardNum = 7;
        //カードの1度のシャッフル時間
        this.speed = 0.6f;
        //カードの移動時間の決定
        this.spanTime = 0.2f;
        //シャッフルする回数の決定
        this.shaffleFinalCount = 20;
        this.announceText.active = false;
        this.buttonBox.active = false;
        this.levelText.GetComponent<Text>().text = "Level9";
        this.levelText.active = true;
        Invoke("startOn", 1.0f);
    }
}
