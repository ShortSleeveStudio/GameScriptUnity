using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using static GameScript.StringWriter;

namespace GameScript
{
    class TranspilingTreeWalker
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

        public static string Transpile(Routines routine, HashSet<string> flagCache)
        {
            // Trim and special case empty
            string code = routine.code.Trim();
            if (code.Length == 0)
            {
                if (!routine.isCondition)
                    return "";
                return "ctx.SetConditionResult(true);";

                // if (!routine.isCondition)
                //     return "new NoopBlockRoutine()";
                // return "new NoopConditionRoutine()";
            }

            // Create parser
            TranspileErrorListener errorListener = new();
            ICharStream stream = CharStreams.fromString(code);
            CSharpRoutineLexer lexer = new CSharpRoutineLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            CSharpRoutineParser parser = new(tokens) { BuildParseTree = true, };
            lexer.RemoveErrorListeners();
            parser.RemoveErrorListeners();
            parser.AddErrorListener(errorListener);

            // Transpile
            try
            {
                IParseTree tree = routine.isCondition ? parser.expression() : parser.routine();
                TranspilingTreeWalker walker = new(flagCache, routine);
                walker.WalkEntry(tree);
                return walker.ToString();
            }
            catch (Exception)
            {
                string message =
                    $"Transpilation error in routine {routine.id} at "
                    + $"line: {errorListener.ErrorLine} "
                    + $"column: {errorListener.ErrorColumn} "
                    + $"message: {errorListener.ErrorMessage}";
                throw new Exception(message);
            }
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

            // // Walk
            // if (m_IsCondition)
            //     AppendNoLine(m_Accumulator, 0, "new ConditionRoutine(() => ");
            // else if (m_IsBlock)
            //     AppendNoLine(m_Accumulator, 0, "new BlockRoutine(() => {");
            // else
            //     AppendNoLine(m_Accumulator, 0, "new ScheduledBlockRoutine(");
            // Walk(tree);
            // if (m_IsCondition)
            //     AppendNoLine(m_Accumulator, 0, ")");
            // else if (m_IsBlock)
            //     AppendNoLine(m_Accumulator, 0, "})");
            // else
            // {
            //     for (int i = 0; i < m_ScheduledBlocks.Count; i++)
            //     {
            //         ScheduledBlockBuilder block = m_ScheduledBlocks[i];
            //         if (i > 0)
            //             AppendNoLine(m_Accumulator, 0, ",");

            //         // Open block
            //         AppendNoLine(m_Accumulator, 0, $"new ScheduledBlockData(");

            //         // Write entry flags
            //         AppendNoLine(m_Accumulator, 0, "new RoutineFlag[]{");
            //         bool firstFlag = true;
            //         foreach (string flag in block.EntryFlags)
            //         {
            //             if (firstFlag)
            //                 firstFlag = false;
            //             else
            //                 AppendNoLine(m_Accumulator, 0, ",");

            //             AppendNoLine(m_Accumulator, 0, $"RoutineFlag.{flag}");
            //         }
            //         AppendNoLine(m_Accumulator, 0, "},");

            //         // Write code parameter
            //         AppendNoLine(m_Accumulator, 0, "(RunnerContext ctx, uint seq) => {");
            //         AppendNoLine(m_Accumulator, 0, block.Code.ToString());
            //         AppendNoLine(m_Accumulator, 0, "}");
            //         AppendNoLine(m_Accumulator, 0, ",");

            //         // Write exit flags
            //         AppendNoLine(m_Accumulator, 0, "new RoutineFlag[]{");
            //         firstFlag = true;
            //         foreach (string flag in block.ExitFlags)
            //         {
            //             if (firstFlag)
            //                 firstFlag = false;
            //             else
            //                 AppendLine(m_Accumulator, 0, ",");

            //             AppendLine(m_Accumulator, 0, $"RoutineFlag.{flag}");
            //         }
            //         AppendNoLine(m_Accumulator, 0, "}");

            //         // Close block
            //         AppendNoLine(m_Accumulator, 0, ")");
            //     }
            //     AppendNoLine(m_Accumulator, 0, ")");
            // }

            // Walk
            if (m_IsCondition)
            {
                AppendNoLine(m_Accumulator, 0, "ctx.SetConditionResult");
                AppendNoLine(m_Accumulator, 0, "(");
            }
            Walk(tree);
            if (m_IsCondition)
            {
                AppendNoLine(m_Accumulator, 0, ");");
            }
        }

        private bool IsBlockOrCondition(IParseTree routineTree)
        {
            if (m_Routine.isCondition)
                return true;
            if (routineTree.ChildCount == 0)
                return true;

            // Get first child
            IParseTree routineChild = routineTree.GetChild(0);

            // This is a terminal node
            if (IsEOF(routineChild))
                return true;

            switch (routineChild)
            {
                case CSharpRoutineParser.Scheduled_blockContext:
                    return false;
                case CSharpRoutineParser.BlockContext:
                    return true;
                default:
                    throw new Exception(
                        $"Routine began with {routineChild.GetType()} instead of scheduled/block"
                    );
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
                            AppendNoLine(m_Accumulator, 0, terminalNode.Symbol.Text);
                        }
                        else
                        {
                            AppendNoLine(m_ScheduledBlocks[^1].Code, 0, terminalNode.Symbol.Text);
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
                case CSharpRoutineParser.Declaration_statementContext declarationContext:
                    HandleDeclaration(declarationContext);
                    break;
                case CSharpRoutineParser.DeclaratorContext declaratorContext:
                    HandleDeclarator(declaratorContext);
                    break;
                case CSharpRoutineParser.NameContext nameContext:
                    HandleName(nameContext);
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
            AppendLine(m_Accumulator, 0, "uint seq = ctx.SequenceNumber;");
            if (m_ScheduledBlocks.Count > 0)
            {
                AppendLine(m_Accumulator, 0, $"ctx.SetBlocksInUse({m_ScheduledBlocks.Count});");
            }
            for (int i = 0; i < m_ScheduledBlocks.Count; i++)
            {
                // Grab Scheduled Block
                ScheduledBlockBuilder scheduledBlock = m_ScheduledBlocks[i];

                // Execution Flag Condition Check
                AppendNoLine(m_Accumulator, 0, $"if (!ctx.HaveBlockFlagsFired({i})");
                if (scheduledBlock.EntryFlags.Count > 0)
                {
                    foreach (string entryFlag in scheduledBlock.EntryFlags)
                    {
                        AppendNoLine(
                            m_Accumulator,
                            0,
                            $" && ctx.IsFlagSet({EditorConstants.k_RoutineFlagEnum}.{entryFlag})"
                        );
                    }
                }
                AppendLine(m_Accumulator, 0, ")");

                // Execution code start
                AppendLine(m_Accumulator, 0, "{");

                // Execution code
                AppendLine(m_Accumulator, 1, $"if (!ctx.IsBlockExecuted({i}))");
                AppendLine(m_Accumulator, 1, "{");
                string[] splits = scheduledBlock.Code.ToString().Split(";");
                for (int j = 0; j < splits.Length; j++)
                {
                    string trimmed = splits[j].TrimEnd();
                    if (string.IsNullOrEmpty(trimmed))
                        continue;
                    AppendLine(m_Accumulator, 2, trimmed + ';');
                }
                AppendLine(m_Accumulator, 2, $"if (ctx.SequenceNumber != seq) return;");
                AppendLine(m_Accumulator, 2, $"ctx.SetBlockExecuted({i});");
                AppendLine(m_Accumulator, 1, "}");
                if (scheduledBlock.ExitFlags.Count > 0)
                {
                    AppendLine(m_Accumulator, 1, $"if (ctx.HaveBlockSignalsFired({i}))");
                    AppendLine(m_Accumulator, 1, "{");
                    foreach (string exitFlag in scheduledBlock.ExitFlags)
                    {
                        AppendLine(
                            m_Accumulator,
                            2,
                            $"ctx.SetFlag({EditorConstants.k_RoutineFlagEnum}.{exitFlag});"
                        );
                    }
                    AppendLine(m_Accumulator, 2, $"ctx.SetBlockFlagsFired({i});");
                    AppendLine(m_Accumulator, 1, "}");
                }

                // Execution code end
                AppendLine(m_Accumulator, 0, "}");
            }
        }
        #endregion

        #region Scheduled Blocks
        private void HandleScheduledBlockOpen(
            CSharpRoutineParser.Scheduled_block_openContext scheduledBlockOpenContext
        )
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
            CSharpRoutineParser.Scheduled_block_closeContext scheduledBlockCloseContext
        )
        {
            // Sanity
            if (m_IsCondition)
            {
                throw new Exception("Scheduled blocks are not allowed in conditions");
            }

            // Add flags to block
            AddFlagListToBlock(
                scheduledBlockCloseContext.flag_list(),
                m_ScheduledBlocks[^1],
                false
            );
        }

        private void AddFlagListToBlock(
            CSharpRoutineParser.Flag_listContext flagList,
            ScheduledBlockBuilder builder,
            bool isEntry
        )
        {
            if (flagList != null && flagList.ChildCount > 0)
            {
                for (int i = 0; i < flagList.ChildCount; i++)
                {
                    // Sanity
                    if (flagList.children[i] is not ITerminalNode identifier)
                    {
                        throw new Exception(
                            "Scheduled block flag list must be a comma separated list"
                        );
                    }

                    // Grab flag name and skip commas
                    string flag = identifier.GetText();
                    if (flag == ",")
                        continue;

                    // Add flag to flag cache
                    m_FlagCache.Add(flag);

                    // Add flag to scheduled block builder (idempotent)
                    if (isEntry)
                        builder.EntryFlags.Add(flag);
                    else
                        builder.ExitFlags.Add(flag);
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
                    case CSharpRoutineParser.LEASE:
                    {
                        EnsureNotCondition("@lease is not allowed in conditions");
                        int currentBlock = m_ScheduledBlocks.Count - 1;
                        ScheduledBlockBuilder block = m_ScheduledBlocks[currentBlock];
                        AppendNoLine(block.Code, 0, $"ctx.AcquireLease({currentBlock}, seq)");
                        break;
                    }
                    case CSharpRoutineParser.NODE:
                    {
                        EnsureNotCondition("@node is not allowed in conditions");
                        AppendNoLine(m_ScheduledBlocks[^1].Code, 0, "ctx.GetCurrentNode(seq)");
                        break;
                    }
                    default:
                    {
                        Walk(child);
                        break;
                    }
                }
            }
            else
                throw new Exception("Encountered literal with multiple children");
        }

        private void EnsureNotCondition(string errorMessage)
        {
            if (m_IsCondition)
                throw new Exception(errorMessage);
        }
        #endregion

        #region Declarations & Names
        private void HandleDeclaration(
            CSharpRoutineParser.Declaration_statementContext declarationContext
        )
        {
            StringBuilder builder = m_IsBlock ? m_Accumulator : m_ScheduledBlocks[^1].Code;
            Walk(declarationContext.type());
            AppendNoLine(builder, 0, " ");
            Walk(declarationContext.declarator_init());
            AppendNoLine(builder, 0, ";");
        }

        private void HandleDeclarator(CSharpRoutineParser.DeclaratorContext declaratorContext) =>
            HandleDeclaratorOrName(declaratorContext.GetText());

        private void HandleName(CSharpRoutineParser.NameContext nameContext) =>
            HandleDeclaratorOrName(nameContext.GetText());

        private void HandleDeclaratorOrName(string str)
        {
            StringBuilder builder = m_IsBlock ? m_Accumulator : m_ScheduledBlocks[^1].Code;
            if (str == "seq")
                AppendNoLine(builder, 0, "_seq");
            else if (str == "ctx")
                AppendNoLine(builder, 0, "_ctx");
            else
                AppendNoLine(builder, 0, str);
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

        #region Helper - Classes/Structs
        private class ScheduledBlockBuilder
        {
            public int ScheduledBlockID { get; private set; }
            public HashSet<string> ExitFlags { get; }
            public HashSet<string> EntryFlags { get; }
            public StringBuilder Code { get; }

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
