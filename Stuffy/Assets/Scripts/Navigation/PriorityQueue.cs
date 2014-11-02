using UnityEngine;
using System.Collections;

public class PriorityQueueNode<T> {
    public T Data { get; private set; }
    public double Priority { get; private set; }
    public long InsertionIndex { get; set; }
    public int QueueIndex { get; set; }


    public PriorityQueueNode(T data, double priority) {
        Data = data;
        Priority = priority;
    }
}

public class PriorityQueue<T> {
    private int _nodeCount;
    private readonly PriorityQueueNode<T>[] _nodes;
    private long _numNodesEverEnqueued;

    public int Count { get { return _nodeCount; } }

    public PriorityQueue(int maxNodes) {
        _nodeCount = 0;
        _nodes = new PriorityQueueNode<T>[maxNodes + 1];
        _numNodesEverEnqueued = 0;
    }
	
    public void Enqueue(T data, double priority) {
        PriorityQueueNode<T> node = new PriorityQueueNode<T>(data, priority);
        _nodeCount++;
        _nodes[_nodeCount] = node;

        node.QueueIndex = _nodeCount;
        node.InsertionIndex = _numNodesEverEnqueued++;
        CascadeUp(_nodes[_nodeCount]);
    }

    public T Dequeue() {
        PriorityQueueNode<T> top = _nodes[1];
        Remove(top);
        return top.Data;
    }

    private void Remove(PriorityQueueNode<T> node) {
        if(!Contains(node)) return;
        
        // wth is this doing...
        if(_nodeCount <= 1) {
            _nodes[1] = null;
            _nodeCount = 0;
            return;
        }
        
        // Make sure node is last in queue.
        bool swapped = false;
        PriorityQueueNode<T> lastNode = _nodes[_nodeCount];
        
        if(node.QueueIndex != _nodeCount) {
            Swap(node, lastNode);
            swapped = true;
        }
        
        _nodeCount--;
        _nodes[node.QueueIndex] = null;
        
        if(swapped)
            OnNodeUpdated(lastNode);
    }
    
    private void OnNodeUpdated(PriorityQueueNode<T> node) {
        int parentIndex = node.QueueIndex / 2;
        PriorityQueueNode<T> parentNode = _nodes[parentIndex];
        
        if(parentIndex > 0 && HasHigherPriority(node, parentNode)) {
            CascadeUp(node);
        } else {
            CascadeDown(node);
        }
    }

    private void CascadeUp(PriorityQueueNode<T> node) {
        int parentIndex = node.QueueIndex / 2;

        while(parentIndex >= 1) {
            PriorityQueueNode<T> parentNode = _nodes[parentIndex];

            if(HasHigherPriority(parentNode, node))
                break;

            Swap(node, parentNode);

            parentIndex = node.QueueIndex / 2;
        }
    }

    private void CascadeDown(PriorityQueueNode<T> node) {
        PriorityQueueNode<T> newParent;
        int finalQueueIndex = node.QueueIndex;

        while(true) {
            newParent = node;
            int childLeftIndex = 2 * finalQueueIndex;

            // Left child is higher priority than current node.
            if(childLeftIndex > _nodeCount) {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;
                break;
            }

            PriorityQueueNode<T> childLeft = _nodes[childLeftIndex];

            if(HasHigherPriority(childLeft, newParent)) 
                newParent = childLeft;

            int childRightIndex = childLeftIndex + 1;

            // Right child is higher priority than current or left nodes.
            if(childRightIndex <= _nodeCount) {
                PriorityQueueNode<T> childRight = _nodes[childRightIndex];

                if(HasHigherPriority(childRight, newParent))
                    newParent = childRight;
            }

            // Either child has higher priority, swap and continue cascading.
            if(newParent != node) {
                _nodes[finalQueueIndex] = newParent;

                int temp = newParent.QueueIndex;
                newParent.QueueIndex = finalQueueIndex;
                finalQueueIndex = temp;
            } else {
                node.QueueIndex = finalQueueIndex;
                _nodes[finalQueueIndex] = node;
                break;
            }
        }
    }

    private bool Contains(PriorityQueueNode<T> node) {
        return _nodes[node.QueueIndex] == node;
    }

    private bool HasHigherPriority(PriorityQueueNode<T> higher, PriorityQueueNode<T> lower) {
        return (higher.Priority < lower.Priority || 
                (higher.Priority == lower.Priority && higher.InsertionIndex < lower.InsertionIndex));
    }

    private void Swap(PriorityQueueNode<T> node1, PriorityQueueNode<T> node2) {
        _nodes[node1.QueueIndex] = node2;
        _nodes[node2.QueueIndex] = node1;

        int temp = node1.QueueIndex;
        node1.QueueIndex = node2.QueueIndex;
        node2.QueueIndex = temp;
    }
}
