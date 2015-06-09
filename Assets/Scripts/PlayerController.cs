using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public Text scoreText;
	public Text winText;
	public Text countdownText;
	public AudioClip coinSound;
	public AudioClip bonusSound;
	public AudioClip hurrySound;
	public AudioClip loserSound;
	public AudioClip winnerSound;
	
	private float _timer;
	private int _count;
	private int _score;
	private Rigidbody _rb;
	private int _totalNumberOfPickups;
	 
	private bool _hasBeenWarned;
	private bool _isGameOver;

	private Vector3 _startPosition;

	private GameObject[] _pickups;
	private GameObject[] _bonusPickups;

	void Start()
	{	
		_pickups = GameObject.FindGameObjectsWithTag ("Pick Up");
		_bonusPickups = GameObject.FindGameObjectsWithTag ("Bonus Pick Up");
		_totalNumberOfPickups = _pickups.Length + _bonusPickups.Length;

        _startPosition = transform.position;
		ResetBoard ();
		_rb = GetComponent<Rigidbody> ();
	}
	 
	void ResetBoard(){
		winText.gameObject.SetActive (false);
		countdownText.color = new Color (255, 255, 255);
		_isGameOver = false;
		_hasBeenWarned = false;
		_timer = 11.0f;
		_count = 0;
		_score = 0;
		winText.text = "";
        scoreText.text = "";
    }
    
    public void Restart(){

		transform.position = _startPosition;

		foreach(var pickUp in _pickups)
		{
			var p = pickUp.GetComponent<BoxCollider>();
			p.isTrigger = true;
			pickUp.SetActive(true);
		}
		
		foreach(var pickUp in _bonusPickups)
		{
			var p = pickUp.GetComponent<CapsuleCollider>();
			p.isTrigger = true;
			pickUp.SetActive(true);
        }

		ResetBoard ();
    } 
    
    void SetWinText(string text){
        winText.text = "YOU LOSE!!!\nPlay Again?";
		winText.gameObject.SetActive (true);
    }
    
    void Update()
	{
		if (!_isGameOver) {

			_timer -= Time.deltaTime;
			
			if (_timer <= 0) {
				_isGameOver = true;

				countdownText.text = "0.00";
				AudioSource.PlayClipAtPoint(loserSound, transform.position);
				// disable all pickups from accepting collisions

				SetWinText("YOU LOSE!!!\nPLAY AGAIN!");

				foreach(var pickUp in _pickups)
				{
					var p = pickUp.GetComponent<BoxCollider>();
					p.isTrigger = false;
				}

				foreach(var pickUp in _bonusPickups)
				{
					var p = pickUp.GetComponent<CapsuleCollider>();
                    p.isTrigger = false;
                }
                
            } else {
                
                if (_timer < 10 && !_hasBeenWarned) {
					AudioSource.PlayClipAtPoint(hurrySound, transform.position);
                    countdownText.color = new Color(255, 0, 0);
					_hasBeenWarned = true;
                }
                
                countdownText.text = _timer.ToString ("0.00");
			}
		}
	}

	void FixedUpdate()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		Vector3 movement = new Vector3 (moveHorizontal, 0, moveVertical);
		_rb.AddForce (movement * speed);
	}

	void CalcGameStatus(Collider other){

		_count++;

		other.gameObject.SetActive (false);

		if (_count == _totalNumberOfPickups) {
			SetWinText("YOU WIN!!!!\\nPlay Again?");
			AudioSource.PlayClipAtPoint(winnerSound, transform.position);
			_isGameOver = true;
        }
    }
    
    void OnTriggerEnter(Collider other) 
	{
		if (other.gameObject.CompareTag ("Pick Up")) {
			AudioSource.PlayClipAtPoint(coinSound, transform.position);
			CalcGameStatus (other);
			SetScore (1);
		} else if (other.gameObject.CompareTag ("Bonus Pick Up")) {
			AudioSource.PlayClipAtPoint(bonusSound, transform.position);
			CalcGameStatus (other);
			SetScore (10);
        }
	}

	void SetScore(int score)
	{
		_score += score;
		scoreText.text = _score.ToString ();
	}
}