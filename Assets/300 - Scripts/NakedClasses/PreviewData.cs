using UnityEngine;
using System.Collections;

public enum ShapeType
{
	Linear,
	Area,
	Cursor,
}

public enum DeployType
{
	Snapping,
	Free,
	Growing,
}

public class PreviewData
{
	public ShapeType shapeType;
	public DeployType deployType;
	public float radius;
	public float range;
	public float timeToDeploy;

}