namespace NQG.TheoryBox
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    public static class NonBlockingConsole
    {
        private static readonly BlockingCollection<Message> m_queue = new BlockingCollection<Message>();

        static NonBlockingConsole()
        {
            var thread = new Thread(
                () =>
                {
                    while (true)
                    {
                        var message = m_queue.Take();
                        ConsoleColor? originalForegroundColor = null;
                        if (message.ForegroundColor.HasValue && Console.ForegroundColor != message.ForegroundColor)
                        {
                            originalForegroundColor = Console.ForegroundColor;
                            Console.ForegroundColor = message.ForegroundColor.Value;
                        }

                        Console.WriteLine(message.Text);

                        if (originalForegroundColor.HasValue)
                            Console.ForegroundColor = originalForegroundColor.Value;
                    }
                })
                {IsBackground = true};
            thread.Start();
        }

        public static void WriteLine(string text, ConsoleColor? foregroundColor = null)
        {
            m_queue.Add(new Message
            {
                ForegroundColor = foregroundColor,
                Text = text
            });
        }

        private class Message
        {
            public ConsoleColor? ForegroundColor
            {
                get;
                set;
            }

            public string Text
            {
                get;
                set;
            }
        }
    }
}
