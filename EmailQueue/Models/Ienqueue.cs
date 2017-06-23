using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailQueue.Models
{
    interface Ienqueue
    {
        Queue<object> enqueue();

        void dequeue(Queue<object> data);
    }
}
