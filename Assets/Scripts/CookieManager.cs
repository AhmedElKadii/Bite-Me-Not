using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CookieManager : MonoBehaviour
{
	public int KILL_MESSAGE_INDEX = 3;

	public GameObject canvas;
	public Animator anim;
	public GameObject killFrame;
	public RawImage textBubble;
	public Texture2D[] messages;

	const int RENDER_DISTANCE = 5;

    void Awake()
    {
		anim.SetBool("isPlaying", true);
		StartCoroutine(PickRandomMessage());
    }

	void Update()
	{
		if (GameManager.Instance.player != null)
		{
			float x = GameManager.Instance.player.transform.position.magnitude;
			float y = transform.position.magnitude;

			float distance = Mathf.Abs(y - x);

			if (distance < RENDER_DISTANCE)
			{
				canvas.SetActive(true);
				canvas.transform.LookAt(GameManager.Instance.player.transform);
				anim.SetBool("isPlaying", true);
			}
			else 
			{
				canvas.SetActive(false);
			}
		}
	}

    IEnumerator PickRandomMessage()
    {
		int index = Random.Range(0, messages.Length);

		if (messages[index] != null)
		{
			textBubble.gameObject.SetActive(true);
			textBubble.texture = messages[index];
		}
		else 
		{
			textBubble.texture = null;
			textBubble.gameObject.SetActive(false);
		}
		
		if (index == KILL_MESSAGE_INDEX)
		{
			anim.SetBool("isPlaying", false);

			yield return new WaitForSeconds(0.5f);

			killFrame.SetActive(true);
		}
    }

	void OnTriggerEnter(Collider other)
	{
		PlayerController player = other.GetComponent<PlayerController>();

		if (player != null)
		{
			player.AddHealth(25);
			Destroy(gameObject);
		}
	}
}
