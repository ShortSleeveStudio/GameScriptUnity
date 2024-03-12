using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Tree;

namespace GameScript
{
    public class TranspilingTreeWalker
    {
        private bool m_IsBlock;
        private bool m_IsCondition;
        private HashSet<string> m_FlagCache;
        private StringBuilder m_Accumulator;
        private List<ScheduledBlockBuilder> m_ScheduledBlocks;
        private Routines m_Routine;

        private TranspilingTreeWalker(HashSet<string> flagCache, Routines routine)
        {
            m_FlagCache = flagCache;
            m_Accumulator = new();
            m_IsCondition = routine.isCondition;
            m_ScheduledBlocks = new();
            m_Routine = routine;
        }

        public static string Transpile(
            IParseTree tree, HashSet<string> flagCache,
            Routines routine)
        {
            TranspilingTreeWalker walker = new(flagCache, routine);
            walker.WalkEntry(tree);
            return walker.ToString();
        }

        public override string ToString() => m_Accumulator.ToString();

        private static bool IsEOF(IParseTree tree)
        {
            return tree is ITerminalNode
                && ((ITerminalNode)tree).Symbol.Type == CSharpRoutineParser.Eof;
        }

        #region Walk
        private void WalkEntry(IParseTree tree)
        {
            // Determine routine type
            m_IsBlock = IsBlockOrCondition(tree);

            // Walk
            if (m_IsCondition)
            {
                m_Accumulator.Append("ctx.SetConditionResult");
                m_Accumulator.Append("(");
            }
            Walk(tree);
            if (m_IsCondition)
            {
                m_Accumulator.Append(");");
            }
        }

        private bool IsBlockOrCondition(IParseTree routineTree)
        {
            if (m_Routine.isCondition) return true;
            if (routineTree.ChildCount == 0) return true;

            // Get first child
            IParseTree routineChild = routineTree.GetChild(0);

            // This is a terminal node
            if (IsEOF(routineChild)) return true;

            switch (routineChild)
            {
                case CSharpRoutineParser.Scheduled_blockContext:
                    return false;
                case CSharpRoutineParser.BlockContext:
                    return true;
                default:
                    throw new Exception(
                        $"Routine began with {routineChild.GetType()} instead of scheduled/block");
            }
        }

        private void Walk(IParseTree routineTree)
        {
            switch (routineTree)
            {
                case IErrorNode errorNode:
                    throw new Exception(errorNode.ToString());
                case ITerminalNode terminalNode:
                    if (terminalNode.Symbol.Type != CSharpRoutineParser.Eof)
                    {
                        if (m_IsBlock)
                        {
                            m_Accumulator.Append(terminalNode.Symbol.Text);
                        }
                        else
                        {
                            m_ScheduledBlocks[^1].Code.Append(terminalNode.Symbol.Text);
                        }
                    }
                    break;
                case IRuleNode ruleNode:
                    HandleRuleNode(ruleNode);
                    break;
                default:
                    throw new Exception($"Unknown node type: {routineTree}");
            }
        }

        private void HandleRuleNode(IRuleNode ruleNode)
        {
            switch (ruleNode)
            {
                case CSharpRoutineParser.RoutineContext routineContext:
                    HandleRoutine(routineContext);
                    break;
                case CSharpRoutineParser.Scheduled_block_openContext scheduledBlockOpenContext:
                    HandleScheduledBlockOpen(scheduledBlockOpenContext);
                    break;
                case CSharpRoutineParser.Scheduled_block_closeContext scheduledBlockCloseContext:
                    HandleScheduledBlockClose(scheduledBlockCloseContext);
                    break;
                case CSharpRoutineParser.LiteralContext literalContext:
                    HandleLiteral(literalContext);
                    break;
                default:
                    HandleNodeDefault(ruleNode);
                    break;
            }
        }
        #endregion

        #region Routine
        private void HandleRoutine(CSharpRoutineParser.RoutineContext routineContext)
        {
            // Walk to tree to gather scheduled blocks
            int childCount = routineContext.ChildCount;
            for (int i = 0; i < childCount; i++)
            {
                Walk(routineContext.GetChild(i));
            }

            // Write routine body
            if (m_ScheduledBlocks.Count > 0)
            {
                m_Accumulator.Append($"ctx.SetBlocksInUse({m_ScheduledBlocks.Count});\n");
            }
            for (int i = 0; i < m_ScheduledBlocks.Count; i++)
            {
                // Grab Scheduled Block
                ScheduledBlockBuilder scheduledBlock = m_ScheduledBlocks[i];

                // Execution Condition Check
                m_Accumulator.Append("if (");

                // Make sure block hasn't already executed
                m_Accumulator.Append($"!ctx.IsBlockExecuted({i})");

                // Check if required flags are set
                foreach (string entryFlag in scheduledBlock.EntryFlags)
                {
                    m_Accumulator.Append(
                        $" && ctx.IsFlagSet((int){Constants.ROUTINE_FLAG_ENUM}.{entryFlag})");
                }

                // Execution condition end
                m_Accumulator.Append(")\n");

                // Execution code start
                m_Accumulator.Append("{\n");

                // Execution code
                string[] splits = scheduledBlock.Code.ToString().Split(";");
                for (int j = 0; j < splits.Length; j++)
                {
                    string trimmed = splits[j].TrimEnd();
                    if (string.IsNullOrEmpty(trimmed)) continue;
                    m_Accumulator.Append("    ");
                    m_Accumulator.Append(trimmed);
                    m_Accumulator.Append(";\n");
                }
                m_Accumulator.Append($"    ctx.SetBlockExecuted({i});\n");
                foreach (string exitFlag in scheduledBlock.ExitFlags)
                {
                    m_Accumulator.Append(
                        $"    ctx.SetFlag((int){Constants.ROUTINE_FLAG_ENUM}.{exitFlag});\n");
                }

                // Execution code end
                m_Accumulator.Append("}\n");
            }
        }
        #endregion

        #region Scheduled Blocks
        private void HandleScheduledBlockOpen(
            CSharpRoutineParser.Scheduled_block_openContext scheduledBlockOpenContext)
        {
            // Sanity
            if (m_IsCondition)
            {
                throw new Exception("Scheduled blocks are not allowed in conditions");
            }

            // Add Scheduled Block Builder
            ScheduledBlockBuilder scheduledBlockBuilder = new(m_ScheduledBlocks.Count);
            m_ScheduledBlocks.Add(scheduledBlockBuilder);

            // Add flags to block
            AddFlagListToBlock(scheduledBlockOpenContext.flag_list(), scheduledBlockBuilder, true);
        }

        private void HandleScheduledBlockClose(
            CSharpRoutineParser.Scheduled_block_closeContext scheduledBlockCloseContext)
        {
            // Sanity
            if (m_IsCondition)
            {
                throw new Exception("Scheduled blocks are not allowed in conditions");
            }

            // Add flags to block
            AddFlagListToBlock(
                scheduledBlockCloseContext.flag_list(), m_ScheduledBlocks[^1], false);
        }

        private void AddFlagListToBlock(
            CSharpRoutineParser.Flag_listContext flagList, ScheduledBlockBuilder builder,
            bool isEntry)
        {
            if (flagList != null && flagList.ChildCount > 0)
            {
                for (int i = 0; i < flagList.ChildCount; i++)
                {
                    // Sanity
                    if (flagList.children[i] is not ITerminalNode identifier)
                    {
                        throw new Exception(
                            "Scheduled block flag list must be a comma separated list");
                    }

                    // Grab flag name and skip commas
                    string flag = identifier.GetText();
                    if (flag == ",") continue;

                    // Add flag to flag cache
                    m_FlagCache.Add(flag);

                    // Add flag to scheduled block builder (idempotent)
                    if (isEntry) builder.EntryFlags.Add(flag);
                    else builder.ExitFlags.Add(flag);
                }
            }
        }
        #endregion

        #region Literals
        private void HandleLiteral(CSharpRoutineParser.LiteralContext literal)
        {
            if (literal.ChildCount == 1)
            {
                IParseTree child = literal.GetChild(0);
                if (!(child is ITerminalNode))
                {
                    throw new Exception("Literal was not a terminal node");
                }
                ITerminalNode terminalChild = (ITerminalNode)child;
                switch (terminalChild.Symbol.Type)
                {
                    case CSharpRoutineParser.SIGNAL:
                        EnsureNotCondition("@sig is not allowed in conditions");
                        int currentBlock = m_ScheduledBlocks.Count - 1;
                        m_ScheduledBlocks[^1].Code.Append(
                            $"ctx.AcquireSignal({currentBlock})");
                        break;
                    case CSharpRoutineParser.NODE:
                        EnsureNotCondition("@node is not allowed in conditions");
                        m_ScheduledBlocks[^1].Code.Append("ctx.GetCurrentNode()");
                        break;
                    default:
                        Walk(child);
                        break;
                }
            }
            else throw new Exception("Encountered literal with multiple children");
        }

        private void EnsureNotCondition(string errorMessage)
        {
            if (m_IsCondition) throw new Exception(errorMessage);
        }
        #endregion

        #region Default Nodes
        private void HandleNodeDefault(IRuleNode ruleNode)
        {
            int childCount = ruleNode.ChildCount;
            for (int i = 0; i < childCount; ++i)
            {
                Walk(ruleNode.GetChild(i));
            }
        }
        #endregion

        #region Helpers - Scheduled Blocks
        private class ScheduledBlockBuilder
        {
            public int SignalCount { get; private set; }
            public int ScheduledBlockID { get; private set; }
            public HashSet<string> ExitFlags { get; }
            public HashSet<string> EntryFlags { get; }
            public StringBuilder Code { get; }

            public int GetThenIncrementSignalCount()
            {
                int count = SignalCount;
                SignalCount++;
                return count;
            }

            public ScheduledBlockBuilder(int blockId)
            {
                Code = new();
                ExitFlags = new();
                EntryFlags = new();
                ScheduledBlockID = blockId;
            }
        }
        #endregion
    }
}
