%start Program

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

// Types
%token INTEGER // integer
%token REAL // real
%token CHAR // char
%token BOOLEAN // boolean
%token INTEGER_REF // Integer
%token REAL_REF // Real
%token CHAR_REF // Char
%token BOOLEAN_REF // Boolean

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
%token EQUAL // =
%token INCREMENT // ++
%token DECREMENT // --
%token MINUS // SUB? -
%token PLUS // SUM? +
%token MULTIPLY // MULT? *
%token DIVIDE // DIV? /
%token MOD // MOD? %
%token LESS_EQUAL // <=
%token GREATER_EQUAL // >=
%token LESS // <
%token GREATER // >
%token NOT_EQUAL // <>
%token AND // and
%token OR // or
%token XOR // xor
%token RANGE // ..


/* RULES SECTION */

%%

// Program : { Declaration }
Program
    : Declaration
    | Program Declaration
    ;

// Declaration : VariableDeclaration | TypeDeclaration | RoutineDeclaration
Declaration
    : SimpleDeclaration
    | RoutineDeclaration
    ;

SimpleDeclaration
    : VariableDeclaration
    | TypeDeclaration
    ;

VariableDeclaration
    : VAR IDENTIFIER TypeTail VariableInitializationTail
    ;

// VariableTypeTail : ':' Type
TypeTail
    : COLON Type
    ;

// VariableInitializationTail : is Expression
VariableInitializationTail
    : IS Expression
    ;

// TypeDeclaration : type Identifier is Type
TypeDeclaration
    : TYPE IDENTIFIER IS Type
    ;

// RoutineDeclaration : routine Identifier ( Parameters ) [ : Type ] is Body end
RoutineDeclaration
    : ROUTINE IDENTIFIER OPEN_PARENTHESIS Parameters CLOSE_PARENTHESIS TypeTail IS Body END
    ;

// Parameters : ParameterDeclaration ParametersTail
Parameters
    : /* empty */
    | ParameterDeclaration ParametersTail
    ;

// ParametersTail : { , ParameterDeclaration }
ParametersTail
    : /* empty */
    | COMMA ParameterDeclaration ParametersTail
    ;

// ParameterDeclaration : Identifier : Type
ParameterDeclaration
    : IDENTIFIER COLON Type
    ;

// Type : PrimitiveType | ArrayType | RecordType | Identifier
Type
    : PrimitiveType | ArrayType | RecordType | IDENTIFIER
    ;

// PrimitiveType: integer | real | boolean | char | Integer | Real | Boolean | Char
PrimitiveType
    : INTEGER | REAL | BOOLEAN | CHAR | INTEGER_REF | REAL_REF | BOOLEAN_REF | CHAR_REF
    ;

// ArrayType : array [ Expression ] Type
ArrayType
    : ARRAY OPEN_BRACKET Expression CLOSE_BRACKET Type
    ;

// RecordType : record RecordVariableDeclarations end
RecordType
    : RECORD RecordVariableDeclarations END
    ;

// RecordVariableDeclarations : { VariableDeclaration }
RecordVariableDeclarations
    : /* empty */
    | RecordVariableDeclarations VariableDeclaration
    ;

// Body : { SimpleDeclaration | Statement }
Body
    : /* empty */
    | Body SimpleDeclaration
    | Body Statement
    ;

// Statement : Assignment | RoutineCall | WhileLoop | ForLoop | ForeachLoop | IfStatement
Statement 
    : Assignment 
    | RoutineCall
    | WhileLoop 
    | ForLoop 
    | ForeachLoop 
    | IfStatement
    ;

// Assignment : ModifiablePrimary := Expression
Assignment 
    : ModifiablePrimary ASSIGNMENT_OP Expression
    ;

// RoutineCall : Identifier RoutineCallParameters
RoutineCall 
    : IDENTIFIER RoutineCallParameters
    ;

// RoutineCallParameters : [ ( Expressions ) ]
RoutineCallParameters
    : /* empty */
    | OPEN_PARENTHESIS Expressions CLOSE_PARENTHESIS
    ;

// Expressions : Expression ExpressionsTail
Expressions
    : Expression ExpressionsTail
    ;

// EXpressionsTail : { , Expression }
ExpressionsTail
    : /* empty */
    | COMMA Expression ExpressionsTail
    ;

// WhileLoop : while Expression loop Body end
WhileLoop 
    : WHILE Expression LOOP Body END
    ;

// ForLoop : for Identifier Range loop Body end
ForLoop 
    : FOR IDENTIFIER Range LOOP Body END
    ;

// Range : in Reverse RangeExpression
Range 
    : IN Reverse RangeExpression
    ;

// Reverse : [ reverse ]
Reverse
    : /* empty */
    | REVERSE
    ;

// RangeExpression : Expression .. Expression
RangeExpression
    : Expression RANGE Expression
    ;

// ForeachLoop : foreach Identifier from ModifiablePrimary loop Body end
ForeachLoop 
    : FOREACH IDENTIFIER FROM ModifiablePrimary LOOP Body END
    ;

// IfStatement : if Expression then Body ElseTail end
IfStatement 
    : IF Expression THEN Body ElseTail END
    ;

// ElseTail : [ else Body ]
ElseTail
    : ELSE Body
    ;

// Expression : Relation ExpressionTail
Expression 
    : Relation ExpressionTail
    ;

// ExpressionTail : { ( and | or | xor ) Relation }
ExpressionTail
    : /* empty */
    | AND Relation ExpressionTail
    | OR Relation ExpressionTail
    | XOR Relation ExpressionTail
    ;

// Relation : Simple RelationTail
Relation 
    : Simple RelationTail 
    ;
    
// RelationTail : [ ( < | <= | > | >= | = | /= ) Simple ]    
RelationTail
    : /* empty */
    | LESS Simple
    | LESS_EQUAL Simple
    | GREATER Simple 
    | GREATER_EQUAL Simple 
    | EQUAL Simple 
    | NOT_EQUAL Simple
    ;

// Simple : Factor SimpleTail
Simple 
    : Factor SimpleTail
    ;

// SimpleTail : { ( * | / | % ) Factor }
SimpleTail
    : /* empty */
    | MULTIPLY Factor SimpleTail 
    | DIVIDE Factor SimpleTail
    | MOD Factor SimpleTail // { ( * | / | % ) Factor }
    ;

// Factor : Summand FactorTail
Factor 
    : Summand FactorTail
    ;

// FactorTail : { ( + | - ) Summand }
FactorTail
    : /* empty */
    | PLUS Summand FactorTail
    | MINUS Summand FactorTail
    ;

// Summand : Primary | ( Expression )
Summand 
    : Primary 
    | OPEN_PARENTHESIS Expression CLOSE_PARENTHESIS
    ;

// Primary : IntegralLiteral | RealLiteral | CharLiteral | StringLiteral | true | false | ModifiablePrimary
Primary 
    : INTEGER_LITERAL
    | REAL_LITERAL
    | CHAR_LITERAL
    | STRING_LITERAL
    | TRUE | FALSE
    | ModifiablePrimary
    ;

// ModifiablePrimary : Identifier ModifiablePrimaryTail 
ModifiablePrimary
    : IDENTIFIER ModifiablePrimaryTail
    ;

// ModifiablePrimaryTail : { . Identifier | [ Expression ] }
ModifiablePrimaryTail
    : /* empty */
    | DOT IDENTIFIER ModifiablePrimaryTail
    | OPEN_BRACKET Expression CLOSE_BRACKET ModifiablePrimaryTail

%%


/* USER SECTION */