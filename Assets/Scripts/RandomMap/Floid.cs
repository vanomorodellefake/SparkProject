using System;
using System.Linq;

public class Floid
{
    public int floid_int(string name1, string name2, Graph graph)
    {
        Graph altgraph = graph;

        for (int k = 0; k < altgraph.Count; k++)
        {
            foreach (Node x in altgraph)
            {
                foreach (var y in x.Links.ToList())
                {
                    foreach (var z in y.Key.Links.ToList())
                    {
                        if (!x.Links.ContainsKey(z.Key))
                        {
                            x.Links.Add(z.Key, y.Value + z.Value);
                        }
                        else if (x.Links[z.Key] > y.Value + z.Value)
                        {
                            x.Links[z.Key] = y.Value + z.Value;
                        }
                    }
                }
            }
        }
        /*
        foreach (Node x in altgraph)
        {
            Console.WriteLine(x.Name + ": ");
            foreach (var link in x.Links)
                Console.WriteLine(link.Key.Name + ", " + link.Value);
        }
        */
        Node start = altgraph.Search(name1);
        Node end = altgraph.Search(name2);
        foreach (var x in start.Links)
            if (x.Key == end)
                return x.Value;
        return -1;
    }

    public (int, int, int) floid(Graph graph)
    {
        Graph altgraph = graph;

        for (int k = 0; k < altgraph.Count; k++)
        {
            foreach (Node x in altgraph)
            {
                foreach (var y in x.Links.ToList())
                {
                    foreach (var z in y.Key.Links.ToList())
                    {
                        if (x != z.Key)
                        {
                            if (!x.Links.ContainsKey(z.Key))
                            {
                                x.Links.Add(z.Key, y.Value + z.Value);
                            }
                            else if (x.Links[z.Key] > y.Value + z.Value)
                            {
                                x.Links[z.Key] = y.Value + z.Value;
                            }
                        }
                    }   
                }
            }
        }
        int maxLenght = -1;
        string currentNode = "";
        string endNode = "";
        foreach (var i in altgraph)
        {
            foreach (var j in i.Links)
            {
                if (maxLenght < j.Value)
                {
                    maxLenght = j.Value;
                    currentNode = i.Name;
                    endNode = j.Key.Name;
                }
                maxLenght = Math.Max(maxLenght, j.Value);
            }
        }
        return (maxLenght, Int32.Parse(currentNode), Int32.Parse(endNode));
    }
}