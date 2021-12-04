using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace login_edit_591_12
{
    class Command
    {
        public string command;
        public int counter;
        public int steps;

        public Command((string command, int counter, int steps) command)
        {
            this.command = command.command;
            this.counter = command.counter;
            this.steps = command.steps;
        }

    }
}
