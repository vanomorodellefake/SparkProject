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

    public Graph floid(Graph graph)
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
        return altgraph;
    }
}