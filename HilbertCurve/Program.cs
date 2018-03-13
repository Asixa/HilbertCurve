using System;
using System.Collections.Generic;
using AcForm;
using SharpDX.DirectInput;

public class Program : DxWindow
{
    [STAThread]
    private static void Main() => new Program().Run(new DxConfiguration("希尔伯特曲线", 512, 512));

    public int order = 1;
    private Node top;
    public static List<Vector2> points = new List<Vector2>(), _points = new List<Vector2>();
    public bool lerping;

    public override void Start()
    {
        backgroundColor = new DxColor(1, 1, 1);
        Next();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(Key.Space) && !lerping) Next();
        points = new List<Vector2>();
        top.ToList();
        AnimationDraw();
    }

    public void Next()
    {
        top = new Node(new Vector2(256, 256), 256);
        top.Generate(order++);
        top.Correct();
    }

    public void AnimationDraw()
    {
        if (lerping)
        {
            lerping = false;
            for (var i = 0; i < _points.Count; i++)
            {
                _points[i] = Vector2.Lerp(_points[i], points[i], FrameDelta * 5);
                if (Math.Abs(_points[i].x - points[i].x) > 0.2f || Math.Abs(_points[i].y - points[i].y) > 0.2f)
                    lerping = true;
            }
        }
        else
        {
            if (_points.Count == 0) _points = points;
            else if (points.Count > _points.Count)
            {
                lerping = true;
                var temp = new List<Vector2>();
                foreach (var t in _points)
                {
                    temp.Add(t);
                    temp.Add(t);
                    temp.Add(t);
                    temp.Add(t);
                }

                _points = temp;
            }
            else
                _points = points;
        }

        DrawPointList(_points);
    }

    public void DrawPointList(List<Vector2> point_list)
    {
        Vector2 p = null;
        foreach (var point in point_list)
        {
            if (p != null) DrawLine((int) p.x, (int) p.y, (int) point.x, (int) point.y, new DxColor(0, 0, 0), 1);
            p = point;
        }
    }

    public class Vector2
    {
        public float x, y;

        public Vector2(float a, float b)
        {
            x = a;
            y = b;
        }

        public static Vector2 operator +(Vector2 left, Vector2 right) =>
            new Vector2(left.x + right.x, left.y + right.y);

        public static Vector2 operator -(Vector2 left, Vector2 right) =>
            new Vector2(left.x - right.x, left.y - right.y);

        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) => t <= 0 ? a :
            (t >= 1) ? b : new Vector2((b.x * t + (1 - t) * a.x), (b.y * t + (1 - t) * a.y));
    }

    public class Node
    {
        public int lenth;
        public Vector2 point;
        public List<Node> nodes;
        public int dir;

        public Node(Vector2 v, int l)
        {
            point = v;
            lenth = l;
        }

        public void ToList()
        {
            if (nodes == null) points.Add(point);
            else
                foreach (var node in nodes)
                    node.ToList();
        }

        public void Generate(int order)
        {
            if (order == 0) return;
            var l = lenth / 2;
            nodes = new List<Node>
            {
                new Node(point + new Vector2(-l, l), l),
                new Node(point + new Vector2(-l, -l), l),
                new Node(point + new Vector2(l, -l), l),
                new Node(point + new Vector2(l, l), l)
            };
            foreach (var node in nodes) node.Generate(order - 1);
        }

        public void Rotate(int add)
        {
            if (dir == 1 || dir == 3)
                add = add == 3 ? 1 :
                    add == 1 ? 3 : add;
            var d = (dir + add) % 4;
            if (nodes == null) return;
            nodes =
                d == 1 ? new List<Node> {nodes[0], nodes[3], nodes[2], nodes[1]} :
                d == 2 ? new List<Node> {nodes[2], nodes[3], nodes[0], nodes[1]} :
                d == 3 ? new List<Node> {nodes[2], nodes[1], nodes[0], nodes[3]} : nodes;
            foreach (var n in nodes) n.dir = d;
        }

        public void Correct()
        {
            if (nodes == null) return;
            nodes[0].Rotate(1);
            nodes[3].Rotate(3);
            nodes[1].Rotate(0);
            nodes[2].Rotate(0);
            foreach (var node in nodes) node.Correct();
        }
    }
}