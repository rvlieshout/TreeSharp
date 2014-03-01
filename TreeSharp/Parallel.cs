#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Collections.Generic;

namespace TreeSharp
{
    /// <summary>
    ///   This class is not yet implemented!
    /// </summary>
    public class Parallel : GroupComposite
    {
        
         public Parallel(params Composite[] children)
            : base(children)
        {
        }

         public Parallel(ContextChangeHandler contextChange, params Composite[] children)
            : this(children)
        {
            ContextChanger = contextChange;
        }

        public override IEnumerable<RunStatus> Execute(object context)
        {
            if (ContextChanger != null)
            {
                context = ContextChanger(context);
            }

            Composite markforremoval = null;

            foreach (Composite node in Children)
            {
                node.Start(context);
                node.Tick(context);
                /*
                while (node.Tick(context) == RunStatus.Running)
                {
                    Selection = node;
                 //   yield return RunStatus.Running;
                }*/

                Selection = null;
                node.Stop(context);

                if (node.LastStatus == RunStatus.Failure)
                {
                    markforremoval = node;
                 //   Children.Remove(node);
                    //yield return RunStatus.Running;
                   // yield break;
                }
            }

            if (markforremoval != null)
                Children.Remove(markforremoval);
            yield return RunStatus.Running;
            yield break;
        }
    /*
        public override IEnumerable<RunStatus> Execute(object context)
        {
            throw new NotImplementedException();
        }*/
    }
}