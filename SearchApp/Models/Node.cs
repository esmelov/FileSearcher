using System.Collections.ObjectModel;
using System.Linq;

namespace SearchApp.Models
{
    public class Node<T>
    {
        public Node(T current)
        {
            Current = current;
            Nodes = new ObservableCollection<Node<T>>();
        }

        public T Current { get; set; }
        public ObservableCollection<Node<T>> Nodes { get; set; }
        public object Tag { get; set; }
        public bool HasChildes => Nodes.Any();
    }
}
