using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KrakenBot2
{
    // Class that handles command queueing for usage at later time
    public class CommandQueue
    {
        private Queue<Command> queue = new Queue<Command>();

        // CommandQueue constructor
        public CommandQueue() { }

        // Performs dequeue operation once
        public void checkQueueOnce()
        {
            Common.other("Checking queue once..");
            if (queue.Count == 0)
                return;
            processCommand(queue.Dequeue());
            Common.other("Queue checked once..");
        }

        // Performs dequeue operation until queue is empty
        public void checkQueueEntirety()
        {
            Common.other("Checking entire queue..");
            if (queue.Count == 0)
                return;
            while (queue.Count != 0)
                processCommand(queue.Dequeue());
            Common.other("Entire queue checked..");
        }

        // Enqueues new command object on basis of command string and data object
        public void addCommand(string command, object data = null)
        {
            queue.Enqueue(new Command(command, data));
        }

        // Processes a single command object
        private void processCommand(Command command)
        {
            switch(command.CommandStr)
            {
                case "update":
                    Common.ChatClient.sendMessage(string.Format("[Auto] Resuming update query..."));
                    Common.initialize("Handle: Update");
                    Update.processUpdateRequest();
                    break;

                case "raffle":
                    Common.Raffle = (Raffle)command.Data;
                    Common.initialize("Raffle object constructed...");
                    Common.Raffle.startRaffle();
                    Common.initialize("Raffle started...");
                    break;
            }
        }

        // Class representing properties found in a command
        private class Command
        {
            private string command;
            private object data;

            public string CommandStr { get { return command; } }
            public object Data { get { return data; } }

            // Command constructor accepts command string and optionally a data object
            public Command(string command, object data = null)
            {
                this.command = command;
                if(data != null)
                    this.data = data;
            }
        }
    }
}
