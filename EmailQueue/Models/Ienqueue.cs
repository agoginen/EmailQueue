using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailQueue.Models
{
    interface Ienqueue
    {
        Queue<emailQueue> enqueue();

        int dequeue(Queue<emailQueue> data);
    }
}
