using UnityEngine;

public class MazeCell : MonoBehaviour
{
	[SerializeField]
	private GameObject _leftWall;


	[SerializeField]
	private GameObject _rightWall;

	[SerializeField]
	private GameObject _frontWall;

	[SerializeField]
	private GameObject _backWall;

	[SerializeField]
	private GameObject _unvisitedBlock;

	public bool IsVisited { get; private set; }

	public int walls = 4;

	public void Visit()
	{
		IsVisited = true;
		_unvisitedBlock.SetActive(false);
		Vector3 scale = transform.localScale;
		float factor = Random.Range(0.995f, 0.999f);
		scale.x *= factor;
		factor = Random.Range(0.995f, 0.999f);
		scale.z *= factor;
		transform.localScale = scale;
	}

	public void ClearLeftWall()
	{ 
		_leftWall.SetActive(false); 
		walls--;
	}

	public void ClearRightWall() 
	{
		_rightWall.SetActive(false); 
		walls--;
	}

	public void ClearFrontWall() 
	{ 
		_frontWall.SetActive(false); 
		walls--;
	}

	public void ClearBackWall() 
	{ 
		_backWall.SetActive(false); 
		walls--;
	}
}
