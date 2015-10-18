using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Vector {
	public int x,y,z;

	public Vector(int x, int y, int z){
		this.x = x;
		this.y = y;
		this.z = z;
	}

	public static Vector operator +(Vector c1, Vector c2) {
		return new Vector(c1.x + c2.x, c1.y + c2.y,c1.z + c2.z);
	}

	public static Vector operator *(Vector c1, int c2) {
		return new Vector(c1.x * c2, c1.y * c2, c1.z * c2);
	}

	public static Vector operator -(Vector c1, Vector c2) {
		return new Vector(c1.x - c2.x, c1.y - c2.y,c1.z - c2.z);
	}

	public override int GetHashCode(){
		return 31 + x ^ y ^ z;
	}

	public override bool Equals(object other){
		if (other == null || other.GetType () != typeof(Vector)) return false;
		Vector vector = (Vector) other;
		return vector.x == x && vector.y == y && vector.z == z;
	}

	public override string ToString() {
		return "(" + x + "," + y + "," + z + ")";
	}

}

public class TrackGenerator : MonoBehaviour {

	public enum Direction{
		FORWARD, BACKWARD, LEFT, RIGHT, UP, DOWN
	}

	public Dictionary<Direction, Vector> directions = new Dictionary<Direction, Vector> ();
	
	public  int length = 100;
	public GameObject crystalPrefab;
	public List<GameObject> cubeSegments = new List<GameObject>();
	private Dictionary<Vector,GameObject> objects = new Dictionary<Vector, GameObject> ();
	private Dictionary<Direction,Direction> inverse = new Dictionary<Direction, Direction> ();
	private GameObject track;

	Direction GetDirection(Vector vector) {
		foreach (KeyValuePair<Direction, Vector> pair in directions)
			if (vector.Equals(pair.Value)) return pair.Key;
		throw new System.Exception("Couldn't get direction for vector " + vector.ToString());
	}

	Vector GetVector(Direction direction) {
		return directions[direction];
	}

	List<Direction> GetPossibleDirections(Vector position){
		List<Direction> directions = new List<Direction> ();
		foreach (Direction direction in System.Enum.GetValues(typeof(Direction))) {
			Vector pos = position + GetVector(direction);
			if(!objects.ContainsKey(pos) && IsPossible(direction, pos)){
				directions.Add(direction);
			}
		}
		return directions;
	}

	private bool IsPossible(Direction direction, Vector position) {
		Direction inverse = this.inverse [direction];
		foreach (Direction dir in System.Enum.GetValues(typeof(Direction))) {
			if(dir == inverse) continue;
			if(objects.ContainsKey(position + GetVector(dir))) return false;
		}
		return true;
	}

	private bool IsTrapPossible(Direction direction, Vector position) {
		if(objects.ContainsKey(position + new Vector(1,1,1))) return false;
		if(objects.ContainsKey(position + new Vector(1,1,-1))) return false;
		if(objects.ContainsKey(position + new Vector(1,-1,1))) return false;
		if(objects.ContainsKey(position + new Vector(1,-1,-1))) return false;
		
		if(objects.ContainsKey(position + new Vector(-1,-1,-1))) return false;
		if(objects.ContainsKey(position + new Vector (-1,1,-1))) return false;
		if(objects.ContainsKey(position + new Vector(-1,-1,1))) return false;
		if(objects.ContainsKey(position + new Vector(-1,1,1))) return false;
		
		if(objects.ContainsKey(position + new Vector(0,-1,-1))) return false;
		if(objects.ContainsKey(position + new Vector(0,-1,1))) return false;
		if(objects.ContainsKey(position + new Vector(0,1,-1))) return false;
		if(objects.ContainsKey(position + new Vector(0,1,1))) return false;
		
		if(objects.ContainsKey(position + new Vector(1,1,0))) return false;
		if(objects.ContainsKey(position + new Vector(-1,1,0))) return false;
		if(objects.ContainsKey(position + new Vector(1,-1,0))) return false;
		if(objects.ContainsKey(position + new Vector(-1,-1,0))) return false;
		
		if(objects.ContainsKey(position + new Vector(1,0,1))) return false;
		if(objects.ContainsKey(position + new Vector(-1,0,1))) return false;
		if(objects.ContainsKey(position + new Vector(1,0,-1))) return false;
		if(objects.ContainsKey(position + new Vector(-1,0,-1))) return false;

		return IsPossible(direction, position);
	}

	// Use this for initialization
	void Start () {
		track = new GameObject ("Track");

		directions.Add (Direction.FORWARD, new Vector (0,0,1));
		directions.Add (Direction.BACKWARD, new Vector (0,0,-1));
		directions.Add (Direction.RIGHT, new Vector (1,0,0));
		directions.Add (Direction.LEFT, new Vector (-1,0,0));
		directions.Add (Direction.UP, new Vector (0,1,0));
		directions.Add (Direction.DOWN, new Vector (0,-1,0));
		inverse.Add (Direction.FORWARD, Direction.BACKWARD);
		inverse.Add (Direction.BACKWARD, Direction.FORWARD);
		inverse.Add (Direction.RIGHT, Direction.LEFT);
		inverse.Add (Direction.LEFT, Direction.RIGHT);
		inverse.Add (Direction.UP, Direction.DOWN);
		inverse.Add (Direction.DOWN, Direction.UP);

		AddSegment(0, new Vector(0,0,0)).transform.FindChild("Back").gameObject.SetActive(false);
		AddSegment(0, new Vector(0,0,1)).transform.FindChild("Front").gameObject.SetActive(false);

		Vector position = new Vector(0,0,1);
		Vector lastPosition = new Vector (0,0,1);


		Direction direction = Direction.FORWARD;
		int count = 0;
		for(int i = 0 ; i < length ; i++){

			List<Direction> possibleDiretions = GetPossibleDirections(position);
			bool lastWasTrap = objects[lastPosition].tag == "Trap";
			bool upOrDown = direction == Direction.UP || direction == Direction.DOWN;
			bool canContinueDirection = count++ < 2 && possibleDiretions.Contains(direction);
			if(!lastWasTrap && (upOrDown || !canContinueDirection)){
				int index = Random.Range(0, possibleDiretions.Count - 1);
				direction = possibleDiretions[index];
				count = 0;
			}
			position += GetVector(direction);
			bool isLastSegment = i == length - 1;
			int type = (!isLastSegment && IsTrapPossible(direction, position))? GetRandomType() : 0;
			GameObject segment = AddSegment(type, position);
			if (isLastSegment) {
				segment.AddComponent<GoalScript>();
			}

			if (segment.tag == "Trap"){
				segment.transform.FindChild("Light").gameObject.GetComponent<Light>().color = new Color (1, 0, 0);
			} else if(segment.tag == "GlassTrap"){
				segment.transform.FindChild("Light").gameObject.GetComponent<Light>().color = new Color (0, 1, 1);
			} else {
				if(Random.value < 0.3){
					Bounds bounds = segment.GetComponent<Collider>().bounds;
					float offset = 10;
					float posX = Random.Range(bounds.min.x + offset, bounds.max.x - offset);
					float posY = Random.Range(bounds.min.y + offset, bounds.max.y - offset);
					float posZ = Random.Range(bounds.min.z + offset, bounds.max.z - offset);
					GameObject crystal = Instantiate (crystalPrefab, new Vector3(posX,posY,posZ), zeroQuaternion) as GameObject;
					crystal.transform.parent = track.transform;
				}
			}

			Vector asd = position - lastPosition;
			Direction dir = GetDirection(asd);
			GameObject lastSegement = objects[lastPosition];

			//Debug.Log("Segment " + position + " dir " + dir);
			switch(dir){
			case Direction.FORWARD:
				segment.transform.FindChild("Front").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Back").gameObject.SetActive(false);
				break;
			case Direction.BACKWARD:
				segment.transform.FindChild("Back").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Front").gameObject.SetActive(false);
				break;
			case Direction.RIGHT:
				segment.transform.FindChild("Left").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Right").gameObject.SetActive(false);
				segment.transform.FindChild("Content").Rotate(new Vector3(0,90,0));
				break;
			case Direction.LEFT:
				segment.transform.FindChild("Right").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Left").gameObject.SetActive(false);
				segment.transform.FindChild("Content").Rotate(new Vector3(0,-90,0));
				break;
			case Direction.UP:
				segment.transform.FindChild("Bottom").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Top").gameObject.SetActive(false);
				segment.transform.FindChild("Content").Rotate(new Vector3(90,0,0));
				break;
			case Direction.DOWN:
				segment.transform.FindChild("Top").gameObject.SetActive(false);
				lastSegement.transform.FindChild("Bottom").gameObject.SetActive(false);
				segment.transform.FindChild("Content").Rotate(new Vector3(-90,0,0));
				break;
			}

			lastPosition = position;
		}

		GameObject lastSegment = objects [lastPosition];
		lastSegment.tag = "Finish";
		lastSegment.transform.FindChild("Light").gameObject.GetComponent<Light>().color = Color.green;

	}
	Quaternion zeroQuaternion = new Quaternion (0, 0, 0, 0);

	private GameObject AddSegment(int index, Vector position){
		GameObject segment = Instantiate (cubeSegments [index], new Vector3(position.x,position.y,position.z)*100, zeroQuaternion) as GameObject;
		objects.Add(position, segment);
		segment.transform.parent = track.transform;
		return segment;
	}

	private int GetRandomType(){
		int index = Random.Range(0, cubeSegments.Count - 1);
		return index;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
