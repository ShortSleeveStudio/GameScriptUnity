//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.13.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from ./CSharpRoutineParser.g4 by ANTLR 4.13.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

using Antlr4.Runtime.Misc;
using IParseTreeListener = Antlr4.Runtime.Tree.IParseTreeListener;
using IToken = Antlr4.Runtime.IToken;

namespace GameScript
{
/// <summary>
/// This interface defines a complete listener for a parse tree produced by
/// <see cref="CSharpRoutineParser"/>.
/// </summary>
[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.13.1")]
[System.CLSCompliant(false)]
public interface ICSharpRoutineParserListener : IParseTreeListener {
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.routine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterRoutine([NotNull] CSharpRoutineParser.RoutineContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.routine"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitRoutine([NotNull] CSharpRoutineParser.RoutineContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterScheduled_block([NotNull] CSharpRoutineParser.Scheduled_blockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitScheduled_block([NotNull] CSharpRoutineParser.Scheduled_blockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block_open"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterScheduled_block_open([NotNull] CSharpRoutineParser.Scheduled_block_openContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block_open"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitScheduled_block_open([NotNull] CSharpRoutineParser.Scheduled_block_openContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block_close"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterScheduled_block_close([NotNull] CSharpRoutineParser.Scheduled_block_closeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.scheduled_block_close"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitScheduled_block_close([NotNull] CSharpRoutineParser.Scheduled_block_closeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBlock([NotNull] CSharpRoutineParser.BlockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBlock([NotNull] CSharpRoutineParser.BlockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement([NotNull] CSharpRoutineParser.StatementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement([NotNull] CSharpRoutineParser.StatementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.compound_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterCompound_statement([NotNull] CSharpRoutineParser.Compound_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.compound_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitCompound_statement([NotNull] CSharpRoutineParser.Compound_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.statement_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterStatement_list([NotNull] CSharpRoutineParser.Statement_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.statement_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitStatement_list([NotNull] CSharpRoutineParser.Statement_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.expression_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_statement([NotNull] CSharpRoutineParser.Expression_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.expression_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_statement([NotNull] CSharpRoutineParser.Expression_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.if_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterIf_statement([NotNull] CSharpRoutineParser.If_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.if_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitIf_statement([NotNull] CSharpRoutineParser.If_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.switch_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitch_statement([NotNull] CSharpRoutineParser.Switch_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.switch_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitch_statement([NotNull] CSharpRoutineParser.Switch_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.switch_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitch_block([NotNull] CSharpRoutineParser.Switch_blockContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.switch_block"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitch_block([NotNull] CSharpRoutineParser.Switch_blockContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.switch_label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSwitch_label([NotNull] CSharpRoutineParser.Switch_labelContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.switch_label"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSwitch_label([NotNull] CSharpRoutineParser.Switch_labelContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.declaration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclaration_statement([NotNull] CSharpRoutineParser.Declaration_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.declaration_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclaration_statement([NotNull] CSharpRoutineParser.Declaration_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.declarator_init"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclarator_init([NotNull] CSharpRoutineParser.Declarator_initContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.declarator_init"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclarator_init([NotNull] CSharpRoutineParser.Declarator_initContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.declarator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterDeclarator([NotNull] CSharpRoutineParser.DeclaratorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.declarator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitDeclarator([NotNull] CSharpRoutineParser.DeclaratorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.break_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterBreak_statement([NotNull] CSharpRoutineParser.Break_statementContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.break_statement"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitBreak_statement([NotNull] CSharpRoutineParser.Break_statementContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.expression_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_list([NotNull] CSharpRoutineParser.Expression_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.expression_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_list([NotNull] CSharpRoutineParser.Expression_listContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_bitwise_or</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_bitwise_or([NotNull] CSharpRoutineParser.Expression_bitwise_orContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_bitwise_or</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_bitwise_or([NotNull] CSharpRoutineParser.Expression_bitwise_orContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_bitwise_and</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_bitwise_and([NotNull] CSharpRoutineParser.Expression_bitwise_andContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_bitwise_and</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_bitwise_and([NotNull] CSharpRoutineParser.Expression_bitwise_andContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_relational_gt</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_relational_gt([NotNull] CSharpRoutineParser.Expression_relational_gtContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_relational_gt</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_relational_gt([NotNull] CSharpRoutineParser.Expression_relational_gtContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_postfix_invoke</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_postfix_invoke([NotNull] CSharpRoutineParser.Expression_postfix_invokeContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_postfix_invoke</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_postfix_invoke([NotNull] CSharpRoutineParser.Expression_postfix_invokeContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_bitwise_xor</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_bitwise_xor([NotNull] CSharpRoutineParser.Expression_bitwise_xorContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_bitwise_xor</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_bitwise_xor([NotNull] CSharpRoutineParser.Expression_bitwise_xorContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_postfix_inc_dec</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_postfix_inc_dec([NotNull] CSharpRoutineParser.Expression_postfix_inc_decContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_postfix_inc_dec</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_postfix_inc_dec([NotNull] CSharpRoutineParser.Expression_postfix_inc_decContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_primary_parenthetical</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_primary_parenthetical([NotNull] CSharpRoutineParser.Expression_primary_parentheticalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_primary_parenthetical</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_primary_parenthetical([NotNull] CSharpRoutineParser.Expression_primary_parentheticalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_additive_add</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_additive_add([NotNull] CSharpRoutineParser.Expression_additive_addContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_additive_add</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_additive_add([NotNull] CSharpRoutineParser.Expression_additive_addContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_shift_right</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_shift_right([NotNull] CSharpRoutineParser.Expression_shift_rightContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_shift_right</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_shift_right([NotNull] CSharpRoutineParser.Expression_shift_rightContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_ternary</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_ternary([NotNull] CSharpRoutineParser.Expression_ternaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_ternary</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_ternary([NotNull] CSharpRoutineParser.Expression_ternaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_equality_not_eq</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_equality_not_eq([NotNull] CSharpRoutineParser.Expression_equality_not_eqContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_equality_not_eq</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_equality_not_eq([NotNull] CSharpRoutineParser.Expression_equality_not_eqContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_logical_or</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_logical_or([NotNull] CSharpRoutineParser.Expression_logical_orContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_logical_or</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_logical_or([NotNull] CSharpRoutineParser.Expression_logical_orContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_primary_literal</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_primary_literal([NotNull] CSharpRoutineParser.Expression_primary_literalContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_primary_literal</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_primary_literal([NotNull] CSharpRoutineParser.Expression_primary_literalContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_relational_ge</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_relational_ge([NotNull] CSharpRoutineParser.Expression_relational_geContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_relational_ge</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_relational_ge([NotNull] CSharpRoutineParser.Expression_relational_geContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_shift_left</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_shift_left([NotNull] CSharpRoutineParser.Expression_shift_leftContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_shift_left</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_shift_left([NotNull] CSharpRoutineParser.Expression_shift_leftContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_equality_eq</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_equality_eq([NotNull] CSharpRoutineParser.Expression_equality_eqContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_equality_eq</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_equality_eq([NotNull] CSharpRoutineParser.Expression_equality_eqContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_primary_name</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_primary_name([NotNull] CSharpRoutineParser.Expression_primary_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_primary_name</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_primary_name([NotNull] CSharpRoutineParser.Expression_primary_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_assignment</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_assignment([NotNull] CSharpRoutineParser.Expression_assignmentContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_assignment</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_assignment([NotNull] CSharpRoutineParser.Expression_assignmentContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_relational_lt</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_relational_lt([NotNull] CSharpRoutineParser.Expression_relational_ltContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_relational_lt</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_relational_lt([NotNull] CSharpRoutineParser.Expression_relational_ltContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_unary</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_unary([NotNull] CSharpRoutineParser.Expression_unaryContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_unary</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_unary([NotNull] CSharpRoutineParser.Expression_unaryContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_additive_sub</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_additive_sub([NotNull] CSharpRoutineParser.Expression_additive_subContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_additive_sub</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_additive_sub([NotNull] CSharpRoutineParser.Expression_additive_subContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_multiplicative_div</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_multiplicative_div([NotNull] CSharpRoutineParser.Expression_multiplicative_divContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_multiplicative_div</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_multiplicative_div([NotNull] CSharpRoutineParser.Expression_multiplicative_divContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_postfix_array_access</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_postfix_array_access([NotNull] CSharpRoutineParser.Expression_postfix_array_accessContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_postfix_array_access</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_postfix_array_access([NotNull] CSharpRoutineParser.Expression_postfix_array_accessContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_logical_and</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_logical_and([NotNull] CSharpRoutineParser.Expression_logical_andContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_logical_and</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_logical_and([NotNull] CSharpRoutineParser.Expression_logical_andContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_multiplicative_mul</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_multiplicative_mul([NotNull] CSharpRoutineParser.Expression_multiplicative_mulContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_multiplicative_mul</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_multiplicative_mul([NotNull] CSharpRoutineParser.Expression_multiplicative_mulContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_multiplicative_mod</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_multiplicative_mod([NotNull] CSharpRoutineParser.Expression_multiplicative_modContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_multiplicative_mod</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_multiplicative_mod([NotNull] CSharpRoutineParser.Expression_multiplicative_modContext context);
	/// <summary>
	/// Enter a parse tree produced by the <c>expression_relational_le</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterExpression_relational_le([NotNull] CSharpRoutineParser.Expression_relational_leContext context);
	/// <summary>
	/// Exit a parse tree produced by the <c>expression_relational_le</c>
	/// labeled alternative in <see cref="CSharpRoutineParser.expression"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitExpression_relational_le([NotNull] CSharpRoutineParser.Expression_relational_leContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.assignment_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterAssignment_operator([NotNull] CSharpRoutineParser.Assignment_operatorContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.assignment_operator"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitAssignment_operator([NotNull] CSharpRoutineParser.Assignment_operatorContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterType([NotNull] CSharpRoutineParser.TypeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitType([NotNull] CSharpRoutineParser.TypeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.primitive_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterPrimitive_type([NotNull] CSharpRoutineParser.Primitive_typeContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.primitive_type"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitPrimitive_type([NotNull] CSharpRoutineParser.Primitive_typeContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterName([NotNull] CSharpRoutineParser.NameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitName([NotNull] CSharpRoutineParser.NameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.normal_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterNormal_name([NotNull] CSharpRoutineParser.Normal_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.normal_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitNormal_name([NotNull] CSharpRoutineParser.Normal_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.special_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterSpecial_name([NotNull] CSharpRoutineParser.Special_nameContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.special_name"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitSpecial_name([NotNull] CSharpRoutineParser.Special_nameContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.flag_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterFlag_list([NotNull] CSharpRoutineParser.Flag_listContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.flag_list"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitFlag_list([NotNull] CSharpRoutineParser.Flag_listContext context);
	/// <summary>
	/// Enter a parse tree produced by <see cref="CSharpRoutineParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void EnterLiteral([NotNull] CSharpRoutineParser.LiteralContext context);
	/// <summary>
	/// Exit a parse tree produced by <see cref="CSharpRoutineParser.literal"/>.
	/// </summary>
	/// <param name="context">The parse tree.</param>
	void ExitLiteral([NotNull] CSharpRoutineParser.LiteralContext context);
}
}
