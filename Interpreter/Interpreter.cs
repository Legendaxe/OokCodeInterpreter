﻿using OokCodeAnalyzer.Core;
namespace Interpreter;

public class Interpreter
{

    public static void Main()
    {
        Interpreter interpreter = new Interpreter("""
            Ook. Ook? Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook! Ook? Ook? Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook? Ook! Ook! Ook? Ook! Ook? Ook.
            Ook! Ook. Ook. Ook? Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook! Ook? Ook? Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook?
            Ook! Ook! Ook? Ook! Ook? Ook. Ook. Ook. Ook! Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook! Ook. Ook! Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook! Ook. Ook. Ook? Ook. Ook? Ook. Ook? Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook! Ook? Ook? Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook? Ook! Ook! Ook? Ook! Ook? Ook. Ook! Ook.
            Ook. Ook? Ook. Ook? Ook. Ook? Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook! Ook? Ook? Ook. Ook. Ook.
            Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook. Ook? Ook! Ook! Ook? Ook! Ook? Ook. Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook.
            Ook? Ook. Ook? Ook. Ook? Ook. Ook? Ook. Ook! Ook. Ook. Ook. Ook. Ook. Ook. Ook.
            Ook! Ook. Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook.
            Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook! Ook!
            Ook! Ook. Ook. Ook? Ook. Ook? Ook. Ook. Ook! Ook.
        """);
        interpreter.ProcessCode();
    }
    
    private enum OpCode
    {
        OpInput,
        OpOutput,
        OpLoopL,
        OpLoopR,
        OpIncCell,
        OpDecCell,
        OpIncPtr,
        OpDecPtr,
    }

    private readonly Dictionary<OpCode, Action> _opCodesFuncs;
    

    private readonly char[] _memoryCells = new char[2000000000];
    private Int32 _pointer = 1800000000;
    private readonly string _ookCode;
    private readonly char[] _ch = new char[1];

    private readonly LinkedList<OpCode> _opCodes = new LinkedList<OpCode>();
    private LinkedListNode<OpCode>? _currentOp;


    public Interpreter(string code)
    {
        _ookCode = code;

        _opCodesFuncs = new Dictionary<OpCode, Action>
        {
            {OpCode.OpInput, () => _memoryCells[_pointer] = (char) Console.Read()},
            {OpCode.OpOutput, () => Console.Write(_memoryCells[_pointer])},
            {OpCode.OpIncCell, () => _memoryCells[_pointer]++ },
            {OpCode.OpDecCell, () => _memoryCells[_pointer]--},
            {OpCode.OpIncPtr, () => _pointer++},
            {OpCode.OpDecPtr, () => _pointer--},
            {OpCode.OpLoopL, LoopFunc},
            {OpCode.OpLoopR, () => {}}
        };
    }

    public void ProcessCode()
    {
        var ookStateMachine = OokStateMachine.Funcs;
        OokStateMachine.State state = OokStateMachine.State.Begin;
        
        using (StringReader stringReader = new StringReader(_ookCode))
        {
            while (stringReader.Read(_ch, 0, 1) >= 1 && state != OokStateMachine.State.Error)
            {
                state = ookStateMachine[state].Item1(_ch[0]);
                switch (state)
                {
                    case OokStateMachine.State.OokDOokD:
                        _opCodes.AddLast(OpCode.OpIncCell);
                        break;
                    case OokStateMachine.State.OokEOokE:
                        _opCodes.AddLast(OpCode.OpDecCell);
                        break;
                    case OokStateMachine.State.OokDOokQ:
                        _opCodes.AddLast(OpCode.OpIncPtr);
                        break;
                    case OokStateMachine.State.OokQOokD:
                        _opCodes.AddLast(OpCode.OpDecPtr);
                        break;
                    case OokStateMachine.State.OokDOokE:
                        _opCodes.AddLast(OpCode.OpInput);
                        break;
                    case OokStateMachine.State.OokEOokD:
                        _opCodes.AddLast(OpCode.OpOutput);
                        break;
                    case OokStateMachine.State.OokQOokE:
                        _opCodes.AddLast(OpCode.OpLoopR);
                        break;
                    case OokStateMachine.State.OokEOokQ:
                        _opCodes.AddLast(OpCode.OpLoopL);
                        break;
                }
            }
            if (state == OokStateMachine.State.Error)
            {
                Console.WriteLine("Error occured");
                return;
            }
        }


        if (_opCodes.First is null)
        {
            return;
        }

        _currentOp = _opCodes.First;
        
        
        while (_currentOp.Next is not null)
        {
            _opCodesFuncs[_currentOp.Value]();
            _currentOp = _currentOp.Next;
        }

        _opCodesFuncs[_currentOp.Value]();
    }
    
    private void LoopFunc()
    {
        if (_currentOp is null)
        {
            return;
        }
        
        LinkedListNode<OpCode> enterOp = _currentOp;
        
        while (_currentOp.Next is not null)
        {
            _currentOp = _currentOp.Next;
            _opCodesFuncs[_currentOp.Value]();
            switch (_currentOp.Value)
            {
                case OpCode.OpLoopR:
                    _currentOp = _memoryCells[_pointer] == 0 ? _currentOp : enterOp.Previous;
                    return;
            }
        }
        
        _opCodesFuncs[_currentOp.Value]();
    }
}