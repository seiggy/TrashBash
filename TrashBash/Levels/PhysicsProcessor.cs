using System;
using System.Collections.Generic;
using System.Threading;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;

namespace TrashBash.Levels
{
    class PhysicsProcessor : IDisposable
    {
        private volatile bool doExit;
        private bool forceSingleThreaded;

        private ManualResetEvent idleEvent = new ManualResetEvent(true);
        private IterateParam iterateParam;

        private List<ObjectLinker> linkList = new List<ObjectLinker>();
        private PhysicsSimulator physicsSimulator;
        private AutoResetEvent processEvent = new AutoResetEvent(false);
        private bool useMultiThreading;

        public PhysicsProcessor(PhysicsSimulator physicsSimulator)
        {
            this.physicsSimulator = physicsSimulator;
            this.physicsSimulator.Iterations = 50;
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.doExit = true;
            this.iterateParam = new IterateParam(new GameTime());
            this.processEvent.Set();
        }

        #endregion

        public void StartThinking()
        {
#if XBOX
            Thread.CurrentThread.SetProcessorAffinity( new int[] { 4 } );
            useMultiThreading = true;
#else
            useMultiThreading = Environment.ProcessorCount > 1;
#endif
            while (!doExit)
            {
                processEvent.WaitOne();
                Think();
            }
        }

        public void Iterate(GameTime gameTime, bool forceSingleThreaded)
        {
            this.forceSingleThreaded = forceSingleThreaded;

            iterateParam = new IterateParam(gameTime);
            processEvent.Set();

            if (!useMultiThreading || forceSingleThreaded)
            {
                DoThinkPhysics();
            }
        }

        public void BlockUntilIdle()
        {
            idleEvent.WaitOne();
        }

        public void AddLink(ObjectLinker link)
        {
            linkList.Add(link);
        }

        private void Think()
        {
            if (useMultiThreading && !forceSingleThreaded)
            {
                DoThinkPhysics();
            }
            forceSingleThreaded = false;
        }

        int elapsedTime = 0;

        private void DoThinkPhysics()
        {
            idleEvent.Reset();
            // elapsedTime += iterateParam.GameTime.ElapsedGameTime.Milliseconds;
            if (elapsedTime < 100)
            {
                //while (elapsedTime > 10)
                //{
                //    physicsSimulator.Update(elapsedTime * .002f);
                //    elapsedTime -= 10;
                //}
                physicsSimulator.Update(iterateParam.GameTime.ElapsedGameTime.Milliseconds * .002f);
            }
            else
            {
                physicsSimulator.Update(100 * .002f);
                elapsedTime = 0;
            }
            
            SynchronizeLinks();
            idleEvent.Set();
        }

        private void SynchronizeLinks()
        {
            foreach (ObjectLinker link in linkList)
            {
                link.Synchronize();
            }
        }
        
        #region Nested Type : IterateParam
        private struct IterateParam
        {
            private GameTime gameTime;
            public IterateParam(GameTime gameTime)
            {
                this.gameTime = gameTime;
            }
            public GameTime GameTime
            {
                get { return this.gameTime; }
            }
        };
        #endregion
    }
}
