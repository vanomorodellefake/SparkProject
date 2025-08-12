using System.Collections;
using System.Collections.Generic;
    public class Node
    {
        public Node(string name)
        {
            Name = name;
        }
        public string Name { get; set; }
        public Dictionary<Node, int> Links { get; set; }
    }
public class Graph : IEnumerable<Node>
{
    List<Node> Nodes = new List<Node>();
    int count;
    public List<Node> ReturnNodes()
    {
        return Nodes;
    }
    public Node Search(string name)
    {
        foreach (Node x in Nodes)
        {
            if (x.Name.Equals(name)) return x;
        }
        throw new KeyNotFoundException($"Node with name '{name}' not found.");
    }

    public void AddNode(string name)
    {
        Node node = new Node(name);
        node.Links = new Dictionary<Node, int>();
        Nodes.Add(node);
        count++;
    }
    public void CreateRoad(string name1, string name2, int lenght, bool flag = true)
    {
        Node node1 = Search(name1), node2 = Search(name2);
        node1.Links.Add(node2, lenght);
        if (flag == true)
        {
            node2.Links.Add(node1, lenght);
        }
    }
    public List<string> ReturnNames()
    {
        List<string> result = new List<string>(); int i = 0;
        foreach (Node x in Nodes)
        {
            result.Add(Nodes[i].Name);
            i++;
        }
        return result;
    }
    public Dictionary<Node, int> ReturnRoads(string name)
    {
        Node node = Search(name);
        return node.Links;
    }
    public int Count { get { return count; } }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)this).GetEnumerator();
    }

    IEnumerator<Node> IEnumerable<Node>.GetEnumerator()
    {
        foreach (Node x in Nodes)
        {
            yield return x;
        }
    }
}