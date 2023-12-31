%namespace PelmeniCompilers.Parser

%output=Parser.cs
%partial 
%sharetokens 
%start Program

%YYSTYPE Node

/* DEFINITION SEQUENCE */

// Identifier
%token IDENTIFIER

// Literals
%token INTEGER_LITERAL
%token REAL_LITERAL
%token CHAR_LITERAL
%token STRING_LITERAL

// Keywords
%token TRUE // true
%token FALSE // false
%token IF // if
%token FOREACH // foreach
%token FROM // from
%token END // end
%token FOR // for
%token LOOP // loop
%token VAR // var
%token IS // is
%token TYPE // type
%token RECORD // record
%token ARRAY // array
%token WHILE // while
%token IN // in
%token REVERSE // reverse
%token THEN // then
%token ELSE // else
%token ROUTINE // routine
%token REF // ref
%token RETURN // return
%token USE // use
%token MODULE // module
%token OPERATOR // operator

// Types
%token INTEGER // integer
%token REAL // real
%token CHAR // char
%token BOOLEAN // boolean

// DELIMITERS
%token DOT // .
%token COMMA // ,
%token COLON // :
%token SEMICOLON // ;
%token ASSIGNMENT_OP // :=
%token CLOSE_PARENTHESIS // )
%token OPEN_PARENTHESIS // (
%token CLOSE_BRACKET // ]
%token OPEN_BRACKET // [

// Operators
%token EQUAL            // =
%token INCREMENT        // ++
%token DECREMENT        // --
%token MINUS            // SUB? -
%token PLUS             // SUM? +
%token MULTIPLY         // MULT? *
%token DIVIDE           // DIV? /
%token MOD              // MOD? %
%token LESS_EQUAL       // <=
%token GREATER_EQUAL    // >=
%token LESS             // <
%token GREATER          // >
%token NOT_EQUAL        // <>
%token NOT              // not
%token AND              // and
%token OR               // or
%token XOR              // xor
%token RANGE            // ..


/* RULES SECTION */

%%
// Program : { Declaration }
Program
    : Module Imports      { $$ = MakeProgram($1, $2); }
    | Program Declaration { $$ = AddToProgram($1, $2); }
    ;

Module
    : MODULE IDENTIFIER SEMICOLON { $$ = MakeModule($2); }
    ;

Imports
    : /* empty */ { $$ = MakeImports(); }
    | USE STRING_LITERAL SEMICOLON Imports { $$ = AddToImports($4, $2); }
    ;

// Declaration : VariableDeclaration | TypeDeclaration | RoutineDeclaration
Declaration
    : SimpleDeclaration     { $$ = $1; }
    | RoutineDeclaration    { $$ = $1; }
    ;

SimpleDeclaration
    : VariableDeclaration   { $$ = $1; }
    | TypeDeclaration       { $$ = $1; }
    ;

// VariableDeclaration : var Identifier [ : Type ] [ is Expression ] ;
VariableDeclaration
    : VAR IDENTIFIER TypeTail VariableInitializationTail SEMICOLON   { $$ = MakeVariableDeclaration($2, $3, $4); } // Identifier, type, value
    | VAR IDENTIFIER IdentifiersTail TypeTail SEMICOLON { $$ = MakeVariablesDeclaration($2, $3, $4); }
    ;

IdentifiersTail
    : /* empty */ { $$ = MakeIdentifiersTail(); }
    | COMMA IDENTIFIER IdentifiersTail { $$ = AddToIdentifiersTail($3, $2); }
    ;

// VariableTypeTail : [ ':' Type ]
TypeTail
    : /* empty */   { $$ = MakeTypeTail(); }
    | COLON Type    { $$ = MakeTypeTail($2); }
    ;

// VariableInitializationTail : is Expression
VariableInitializationTail
    : /* empty */ { $$ = MakeVariableInitializationTail(); }
    | IS Expression { $$ = MakeVariableInitializationTail($2); }
    ;

// TypeDeclaration : type Identifier is Type
TypeDeclaration
    : TYPE IDENTIFIER IS Type SEMICOLON  { $$ = MakeTypeDeclaration($2, $4); } // Identifier, type
    ;

// RoutineDeclaration : routine Identifier ( Parameters ) [ : Type ] is Body end
RoutineDeclaration
    : ROUTINE IDENTIFIER OPEN_PARENTHESIS Parameters CLOSE_PARENTHESIS TypeTail IS Body END { $$ = MakeRoutineDeclaration($2, $4, $6, $8); } // Identifier, parameters, type, body
    ;

// Parameters : ParameterDeclaration ParametersTail
Parameters
    : /* empty */                           { $$ = MakeParameters(); }
    | ParameterDeclaration ParametersTail   { $$ = MakeParameters($1, $2); } // Parameter, tail
    ;

// ParametersTail : { , ParameterDeclaration }
ParametersTail
    : /* empty */                               { $$ = MakeParametersTail(); }
    | COMMA ParameterDeclaration ParametersTail { $$ = AddToParametersTail($3, $2); } // Tail, Parameter
    ;

// ParameterDeclaration : Identifier : Type
ParameterDeclaration
    : IDENTIFIER TypeTail { $$ = MakeParameterDeclaration($1, $2); } // Identifier, type
    ;

// Type : PrimitiveType | ArrayType | RecordType | Identifier
Type
    : PrimitiveType { $$ = $1; }
    | ArrayType     { $$ = $1; }
    | RecordType    { $$ = $1; }
    | RefType       { $$ = $1; }
    | IDENTIFIER    { $$ = $1; }
    ;

// PrimitiveType: integer | real | boolean | char | Integer | Real | Boolean | Char
PrimitiveType
    : INTEGER       { $$ = $1; }
    | REAL          { $$ = $1; }
    | BOOLEAN       { $$ = $1; }
    | CHAR          { $$ = $1; }
    ;

// ArrayType : array [ Expression ] Type
ArrayType
    : ARRAY OPEN_BRACKET Expression CLOSE_BRACKET Type { $$ = MakeArrayType($5, $3); } // Type, size
    | ARRAY OPEN_BRACKET CLOSE_BRACKET Type { $$ = MakeArrayType($4); }
    ;

//CompoundSize
//    : Expression { $$ = MakeCompoundSize($1); }
//    ;

//CompoundSizeTail
//    : /* empty */ { $$ = MakeCompoundSizeTail(); }
//    | COMMA Expression CompoundSizeTail { $$ = AddToCompoundSizeTail($3, $2); }
//    ;

// RecordType : record RecordVariableDeclarations end
RecordType
    : RECORD RecordVariableDeclarations END { $$ = MakeRecordType($2); } // Variables
    ;

RefType
    : REF PrimitiveType { $$ = MakeRef($2); }
    ;

// RecordVariableDeclarations : { VariableDeclaration }
RecordVariableDeclarations
    : /* empty */                                       { $$ = MakeRecordVariableDeclarations(); }
    | RecordVariableDeclarations VariableDeclaration    { $$ = AddToRecordVariableDeclarations($1, $2); } // Declarations, Declaration
//    | RecordVariableDeclarations RoutineDeclaration     { $$ = AddToRecordVariableDeclarations($1, $2); }
//    | RecordVariableDeclarations OperatorDeclaration    { $$ = AddToRecordVariableDeclarations($1, $2); }
    ;

//OperatorDeclaration
//    : OPERATOR Operator OPEN_PARENTHESIS Parameters CLOSE_PARENTHESIS TypeTail IS Body END { $$ = MakeOperatorDeclaration($2, $4, $6, $8); }
//    ;

//Operator
//    :   EQUAL { $$ = MakeBinaryOperator($1); }
//    |   INCREMENT { $$ = MakeUnaryOperator($1); } 
//    |   DECREMENT { $$ = MakeUnaryOperator($1); }  
//    |   MINUS { $$ = MakeBinaryOperator($1); }   
//    |   PLUS { $$ = MakeBinaryOperator($1); }     
//    |   MULTIPLY  { $$ = MakeBinaryOperator($1); }   
//    |   DIVIDE  { $$ = MakeBinaryOperator($1); }     
//    |   MOD      { $$ = MakeBinaryOperator($1); }    
//    |   LESS_EQUAL  { $$ = MakeBinaryOperator($1); } 
//    |   GREATER_EQUAL{ $$ = MakeBinaryOperator($1); }
//    |   LESS      { $$ = MakeBinaryOperator($1); }   
//    |   GREATER   { $$ = MakeBinaryOperator($1); }   
//    |   NOT_EQUAL { $$ = MakeBinaryOperator($1); }   
//    |   NOT     { $$ = MakeUnaryOperator($1); }     
//    |   AND      { $$ = MakeBinaryOperator($1); }    
//    |   OR      { $$ = MakeBinaryOperator($1); }     
//    |   XOR     { $$ = MakeBinaryOperator($1); }     
//    |   RANGE    { $$ = MakeBinaryOperator($1); }
//    |   CallOperator { $$ = MakeUnaryOperator($1); }
//    |   ArrayAccessOperator { $$ = MakeUnaryOperator($1); }
//    ;

//CallOperator
//    :   OPEN_PARENTHESIS CLOSE_PARENTHESIS { $$ = MakeCallOperator(); }
//    ;

//ArrayAccessOperator
//    :   OPEN_BRACKET CLOSE_BRACKET { $$ = MakeArrayAccessOperator(); }
//    ;

// Body : { SimpleDeclaration | Statement }
Body
    : /* empty */               { $$ = MakeBody(); }
    | Body SimpleDeclaration    { $$ = AddSimpleDeclarationToBody($1, $2); } // Body, SimpleDeclaration
    | Body Statement            { $$ = AddStatementToBody($1, $2); }
    ;

// Statement : Assignment | RoutineCall | WhileLoop | ForLoop | ForeachLoop | IfStatement
Statement 
    : Assignment        SEMICOLON   { $$ = $1; } 
    | Increment         SEMICOLON   { $$ = $1; }
    | Decrement         SEMICOLON   { $$ = $1; }
    | RoutineCall       SEMICOLON   { $$ = $1; }
    | Return            SEMICOLON   { $$ = $1; }
    | WhileLoop                     { $$ = $1; }
    | ForLoop                       { $$ = $1; }
    | ForeachLoop                   { $$ = $1; }
    | IfStatement                   { $$ = $1; }
    ;

// Assignment : ModifiablePrimary := Expression
Assignment 
    : ModifiablePrimary ASSIGNMENT_OP Expression    { $$ = MakeAssignment($1, $3); } // ModPrimary, Expression
    ;

Increment
    : ModifiablePrimary INCREMENT  { $$ = MakeIncrement($1); } // Identifier 
    ;

Decrement
    : ModifiablePrimary DECREMENT  { $$ = MakeDecrement($1); } // Identifier
    ;

// RoutineCall : Identifier RoutineCallParameters
RoutineCall 
    : IDENTIFIER RoutineCallParameters  { $$ = MakeRoutineCall($1, $2); } // Identifier, Parameters
    ;

Return
    : RETURN Expression { $$ = MakeReturn($2); }
    ;

// RoutineCallParameters : [ ( Expressions ) ]
RoutineCallParameters
    : OPEN_PARENTHESIS             CLOSE_PARENTHESIS    { $$ = MakeRoutineCallParameters(); }
    | OPEN_PARENTHESIS Expressions CLOSE_PARENTHESIS    { $$ = MakeRoutineCallParameters($2); } // Expressions
    ;

// Expressions : Expression ExpressionsTail
Expressions
    : Expression ExpressionsTail    { $$ = MakeExpressions($1, $2); } // Expression, ExprssionTail
    ;

// EXpressionsTail : { , Expression }
ExpressionsTail
    : /* empty */                       { $$ = MakeExpressionTail(); }
    | COMMA Expression ExpressionsTail  { $$ = AddToExpressionTail($3, $2); } // ExpressionTail, Expression
    ;

// WhileLoop : while Expression loop Body end
WhileLoop 
    : WHILE Expression LOOP Body END    { $$ = MakeWhileLoop($2, $4); } // Expression, Body
    ;

// ForLoop : for Identifier Range loop Body end
ForLoop 
    : FOR IDENTIFIER Range LOOP Body END    { $$ = MakeForLoop($2, $3, $5); } // Identifier, Range, Body
    ;

// Range : in Reverse RangeExpression
Range 
    : IN Reverse RangeExpression    { $$ = MakeRange($2, $3); } // Reverse, RangeExpr
    ;

// Reverse : [ reverse ]
Reverse
    : /* empty */   { $$ = MakeReverse(); }
    | REVERSE       { $$ = MakeReverse($1); }
    ;

// RangeExpression : Expression .. Expression
RangeExpression
    : Expression RANGE Expression   { $$ = MakeRangeExpression($1, $3); } // Expression, Expression
    ;

// ForeachLoop : foreach Identifier from ModifiablePrimary loop Body end
ForeachLoop 
    : FOREACH IDENTIFIER FROM ModifiablePrimary LOOP Body END   { $$ = MakeForEachLoop($2, $4, $6); } // Identifier, ModPrimary, Body
    ;

// IfStatement : if Expression then Body ElseTail end
IfStatement 
    : IF Expression THEN Body ElseTail END  { $$ = MakeIfStatement($2, $4, $5); } // Expression, Body, ElseTail
    ;

// ElseTail : [ else Body ]
ElseTail
    : /* empty */   { $$ = MakeElse(); }
    | ELSE Body     { $$ = MakeElse($2); }
    ;

// Expression : Relation ExpressionTail
Expression
    : OrExpression { $$ = MakeExpression($1); }
    ;

AndExpression
    : Relation { $$ = MakeAndExpression($1); }
    | AndExpression AND Relation { $$ = MakeAndExpression($2, $1, $3); }
    ;

OrExpression
    : AndExpression { $$ = MakeOrExpression($1); }
    | OrExpression OR AndExpression { $$ = MakeOrExpression($2, $1, $3); }
    ;

// ExpressionTail : { ( and | or | xor ) Relation }
//ExpressionTail
//    : /* empty */ { $$ = MakeExpressionTail(); }
//    | AND   Relation ExpressionTail { $$ = AddToExpressionTail($1, $2, $3); } // Operation (AND), Relation, ExpressionTail
//    | OR    Relation ExpressionTail { $$ = AddToExpressionTail($1, $2, $3); } // Operation (OR), Relation, ExpressionTail
//   | XOR   Relation ExpressionTail { $$ = AddToExpressionTail($1, $2, $3); } // Operation (XOR), Relation, ExpressionTail
//    ;

// Relation : Simple RelationTail
Relation
    : Factor { $$ = MakeRelation($1); } // Simple, RelationTail
    | Factor LESS          Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (<), Simple
    | Factor LESS_EQUAL    Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (<=), Simple
    | Factor GREATER       Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (>), Simple
    | Factor GREATER_EQUAL Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (>=), Simple
    | Factor EQUAL         Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (=), Simple
    | Factor NOT_EQUAL     Factor  { $$ = MakeRelation($2, $1, $3); } // Operator (<>), Simple
    | NOT Relation { $$ = MakeRelation($1, $2); }
    ;
    
// RelationTail : [ ( < | <= | > | >= | = | /= ) Simple ]    
//RelationTail
//    : /* empty */           { $$ = MakeRelationTail();       }
//    | LESS          Factor  { $$ = MakeRelationTail($1, $2); } // Operator (<), Simple
//    | LESS_EQUAL    Factor  { $$ = MakeRelationTail($1, $2); } // Operator (<=), Simple
//    | GREATER       Factor  { $$ = MakeRelationTail($1, $2); } // Operator (>), Simple
//    | GREATER_EQUAL Factor  { $$ = MakeRelationTail($1, $2); } // Operator (>=), Simple
//    | EQUAL         Factor  { $$ = MakeRelationTail($1, $2); } // Operator (=), Simple
//    | NOT_EQUAL     Factor  { $$ = MakeRelationTail($1, $2); } // Operator (<>), Simple
//   ;

// Simple : Factor SimpleTail
Simple 
    : Summand { $$ = MakeSimple($1); } // Factor, SimpleTail
    | Summand MULTIPLY Simple { $$ = MakeSimple($2, $1, $3); }
    | Summand DIVIDE Simple { $$ = MakeSimple($2, $1, $3); }
    | Summand MOD Simple { $$ = MakeSimple($2, $1, $3); }
    ;

// SimpleTail : { ( * | / | % ) Factor }
//SimpleTail
//    : /* empty */                    { $$ = MakeSimpleTail();            }
//    | MULTIPLY  Summand SimpleTail   { $$ = AddToSimpleTail($1, $2, $3); } // Operator (*), Factor, SimpleTail
//    | DIVIDE    Summand SimpleTail   { $$ = AddToSimpleTail($1, $2, $3); } // Operator (/), Factor, SimpleTail
//    | MOD       Summand SimpleTail   { $$ = AddToSimpleTail($1, $2, $3); } // Operator (%), Factor, SimpleTail
//    ;

// Factor : Summand FactorTail
Factor 
    : Simple { $$ = MakeFactor($1); } // Summand, FactorTail
    | Simple PLUS Factor { $$ = MakeFactor($2, $1, $3); }
    | Simple MINUS Factor { $$ = MakeFactor($2, $1, $3); }
    ;

// FactorTail : { ( + | - ) Summand }
//FactorTail
//    : /* empty */               { $$ = MakeFactorTail(); }
//    | PLUS  Simple FactorTail  { $$ = AddToFactorTail($1, $2, $3); } // Operator (+), Summand, FactorTail
//    | MINUS Simple FactorTail  { $$ = AddToFactorTail($1, $2, $3); } // Operator (+), Summand, FactorTail
//    ;

// Summand : Primary | ( Expression )
Summand 
    : Sign             Primary                      { $$ = MakeSummand($1, $2); } // Primary
    | OPEN_PARENTHESIS Expression CLOSE_PARENTHESIS { $$ = MakeSummand($2); } // Expression
    ;

Sign
    : /* empty */ { $$ = MakeSign(); }
    | MINUS       { $$ = MakeSign($1); }
    ;

// Primary : IntegralLiteral | RealLiteral | CharLiteral | StringLiteral | true | false | ModifiablePrimary
Primary 
    : INTEGER_LITERAL   { $$ = $1; }
    | REAL_LITERAL      { $$ = $1; }
    | CHAR_LITERAL      { $$ = $1; }
    | STRING_LITERAL    { $$ = $1; }
    | TRUE              { $$ = $1; }
    | FALSE             { $$ = $1; }
    | ModifiablePrimary { $$ = $1; }
    | RoutineCall       { $$ = $1; }
    ;

// ModifiablePrimary : Identifier ModifiablePrimaryTail 
ModifiablePrimary
    : IDENTIFIER ModifiablePrimaryTail  { $$ = MakeModifiablePrimary($1, $2); } // Identifier, ModPrimaryTail
    ;

// ModifiablePrimaryTail : { . Identifier | [ Expression ] }
ModifiablePrimaryTail
    : /* empty */                           { $$ = MakeModifiablePrimaryTail(); }
    | ModifiablePrimaryTail MemberAccess   { $$ = AddToModifiablePrimaryTail($2, $1); } // MemberAccess, ModifiablePrimaryTail ??
    | ModifiablePrimaryTail ArrayAccess  { $$ = AddToModifiablePrimaryTail($2, $1); } // ArrayAccess, ModifiablePrimaryTail ??
    ;

MemberAccess
    : DOT IDENTIFIER    { $$ = MakeMemberAccess($2); } // Identifier
    ;

// MemberCall
//    : DOT RoutineCall { $$ = MakeMemberCall($2); }
//    ;

ArrayAccess
    : OPEN_BRACKET Expression CLOSE_BRACKET { $$ = MakeArrayAccess($2); } // Expression
    ;

%%
