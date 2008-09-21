using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Indy.IL2CPU.Assembler;
using Indy.IL2CPU.Assembler.X86;
using CPU = Indy.IL2CPU.Assembler;
using CPUx86 = Indy.IL2CPU.Assembler.X86;
using Asm = Indy.IL2CPU.Assembler;
using Assembler = Indy.IL2CPU.Assembler.Assembler;

namespace Indy.IL2CPU.IL.X86
{
	[OpCode(OpCodeEnum.Newobj)]
	public class Newobj : Op
	{
		public MethodBase CtorDef;
		public string CurrentLabel;
		public int ILOffset;
		public MethodInformation MethodInformation;

        public static void ScanOp(MethodBase aCtor) {
            Call.ScanOp(aCtor);
            Call.ScanOp(GCImplementationRefs.AllocNewObjectRef);
            Call.ScanOp(CPU.Assembler.CurrentExceptionOccurredRef);
            Call.ScanOp(GCImplementationRefs.IncRefCountRef);
        }

        public static void ScanOp(ILReader aReader, MethodInformation aMethodInfo, SortedList<string, object> aMethodData) {
            var xCtorDef = aReader.OperandValueMethod;
            ScanOp(xCtorDef);
        }

	    public Newobj(ILReader aReader,
					  MethodInformation aMethodInfo)
			: base(aReader,
				   aMethodInfo)
		{
			CtorDef = aReader.OperandValueMethod;
			CurrentLabel = GetInstructionLabel(aReader);
			MethodInformation = aMethodInfo;
			ILOffset = aReader.Position;
	        mNextLabel = GetInstructionLabel(aReader.NextPosition);
		}

	    private string mNextLabel;

		public override void DoAssemble()
		{
			Assemble(Assembler,
					 CtorDef,
					 Engine.RegisterType(CtorDef.DeclaringType),
					 CurrentLabel,
					 MethodInformation,
                     ILOffset, mNextLabel);
		}

		public static void Assemble(Assembler.Assembler aAssembler,
									MethodBase aCtorDef,
									int aTypeId,
									string aCurrentLabel,
									MethodInformation aCurrentMethodInformation,
									int aCurrentILOffset,
            string aNextLabel)
		{
			if (aCtorDef != null)
			{
				Engine.QueueMethod(aCtorDef);
			}
			else
			{
				throw new ArgumentNullException("aCtorDef");
			}
			var xTypeInfo = Engine.GetTypeInfo(aCtorDef.DeclaringType);
			MethodInformation xCtorInfo = Engine.GetMethodInfo(aCtorDef,
																   aCtorDef,
																   Label.GenerateLabelName(aCtorDef),
																   Engine.GetTypeInfo(aCtorDef.DeclaringType),
																   aCurrentMethodInformation.DebugMode);
			if (xTypeInfo.NeedsGC)
			{
				int xObjectSize = ObjectUtilities.GetObjectStorageSize(aCtorDef.DeclaringType);
				for (int i = 1; i < xCtorInfo.Arguments.Length; i++)
				{
					aAssembler.StackContents.Pop();
				}
				Engine.QueueMethod(GCImplementationRefs.AllocNewObjectRef);
				Engine.QueueMethod(GCImplementationRefs.IncRefCountRef);
				int xExtraSize = 20;
				new Pushd("0" + (xObjectSize + xExtraSize).ToString("X").ToUpper() + "h");
				new Assembler.X86.Call(Label.GenerateLabelName(GCImplementationRefs.AllocNewObjectRef));
				Engine.QueueMethod(CPU.Assembler.CurrentExceptionOccurredRef);
				//new CPUx86.Pushd(CPUx86.Registers.EAX);
				new Test(Registers.ECX,
						 2);
				//new CPUx86.JumpIfEquals(aCurrentLabel + "_NO_ERROR_1");
				//for (int i = 1; i < xCtorInfo.Arguments.Length; i++) {
				//    new CPUx86.Add(CPUx86.Registers.ESP, (xCtorInfo.Arguments[i].Size % 4 == 0 ? xCtorInfo.Arguments[i].Size : ((xCtorInfo.Arguments[i].Size / 4) * 4) + 1).ToString());
				//}
				//new CPUx86.Add("esp", "4");
				//Call.EmitExceptionLogic(aAssembler, aCurrentMethodInformation, aCurrentLabel + "_NO_ERROR_1", false);
				//new CPU.Label(aCurrentLabel + "_NO_ERROR_1");
				new Pushd(Registers.AtESP);
				new Pushd(Registers.AtESP);
				new Pushd(Registers.AtESP);
				new Pushd(Registers.AtESP);
				new Assembler.X86.Call(Label.GenerateLabelName(GCImplementationRefs.IncRefCountRef));
				//new CPUx86.Test("ecx", "2");
				//new CPUx86.JumpIfEquals(aCurrentLabel + "_NO_ERROR_2");
				//for (int i = 1; i < xCtorInfo.Arguments.Length; i++) {
				//    new CPUx86.Add(CPUx86.Registers.ESP, (xCtorInfo.Arguments[i].Size % 4 == 0 ? xCtorInfo.Arguments[i].Size : ((xCtorInfo.Arguments[i].Size / 4) * 4) + 1).ToString());
				//}
				//new CPUx86.Add("esp", "16");
				//Call.EmitExceptionLogic(aAssembler, aCurrentMethodInformation, aCurrentLabel + "_NO_ERROR_2", false);
				//new CPU.Label(aCurrentLabel + "_NO_ERROR_2");
				new Assembler.X86.Call(Label.GenerateLabelName(GCImplementationRefs.IncRefCountRef));
				//new CPUx86.Test("ecx", "2");
				//new CPUx86.JumpIfEquals(aCurrentLabel + "_NO_ERROR_3");
				//for (int i = 1; i < xCtorInfo.Arguments.Length; i++) {
				//    new CPUx86.Add(CPUx86.Registers.ESP, (xCtorInfo.Arguments[i].Size % 4 == 0 ? xCtorInfo.Arguments[i].Size : ((xCtorInfo.Arguments[i].Size / 4) * 4) + 1).ToString());
				//}
				//new CPUx86.Add("esp", "12");
				//Call.EmitExceptionLogic(aAssembler, aCurrentMethodInformation, aCurrentLabel + "_NO_ERROR_3", false);
				//new CPU.Label(aCurrentLabel + "_NO_ERROR_3");
				int xObjSize = 0;
				int xGCFieldCount = (from item in Engine.GetTypeFieldInfo(aCtorDef,
																		  out xObjSize).Values
									 where item.NeedsGC
									 select item).Count();
				new Assembler.X86.Pop(Registers.EAX);
				new Move("dword",
						 Registers.AtEAX,
						 "0" + aTypeId.ToString("X") + "h");
				new Move("dword",
						 "[eax + 4]",
						 "0" + InstanceTypeEnum.NormalObject.ToString("X") + "h");
				new Move("dword",
						 "[eax + 8]",
						 "0x" + xGCFieldCount.ToString("X"));
				int xSize = (from item in xCtorInfo.Arguments
							 select item.Size + (item.Size % 4 == 0
													 ? 0
													 : (4 - (item.Size % 4)))).Take(xCtorInfo.Arguments.Length - 1).Sum();
				for (int i = 1; i < xCtorInfo.Arguments.Length; i++)
				{
					new Comment(String.Format("Arg {0}: {1}", i, xCtorInfo.Arguments[i].Size));
					for (int j = 0; j < xCtorInfo.Arguments[i].Size; j += 4)
					{
						new Pushd("[esp + 0x" + (xSize + 4).ToString("X") + "]");
					}
				}
				new Assembler.X86.Call(Label.GenerateLabelName(aCtorDef));
				new Test(Registers.ECX,
						 2);
				new JumpIfEqual(aCurrentLabel + "_NO_ERROR_4");
				for (int i = 1; i < xCtorInfo.Arguments.Length; i++)
				{
					new Assembler.X86.Add(Registers.ESP,
										  (xCtorInfo.Arguments[i].Size % 4 == 0
											   ? xCtorInfo.Arguments[i].Size
											   : ((xCtorInfo.Arguments[i].Size / 4) * 4) + 1).ToString());
				}
				new Assembler.X86.Add(Registers.ESP,
									  "4");
				foreach (StackContent xStackInt in aAssembler.StackContents)
				{
					new Assembler.X86.Add(Registers.ESP,
										  xStackInt.Size.ToString());
				}
				Call.EmitExceptionLogic(aAssembler,
										aCurrentILOffset,
										aCurrentMethodInformation,
										aCurrentLabel + "_NO_ERROR_4",
										false,
										null);
				new Label(aCurrentLabel + "_NO_ERROR_4");
				new Assembler.X86.Pop(Registers.EAX);
				//				aAssembler.StackSizes.Pop();
				//	new CPUx86.Add(CPUx86.Registers.ESP, "4");
				for (int i = 1; i < xCtorInfo.Arguments.Length; i++)
				{
					new Assembler.X86.Add(Registers.ESP,
										  (xCtorInfo.Arguments[i].Size % 4 == 0
											   ? xCtorInfo.Arguments[i].Size
											   : ((xCtorInfo.Arguments[i].Size / 4) * 4) + 1).ToString());
				}
				new Push(Registers.EAX);
				aAssembler.StackContents.Push(new StackContent(4,
															   aCtorDef.DeclaringType));
			}
			else
			{
				/*
                 * Current sitation on stack:
                 *   $ESP       Arg
                 *   $ESP+..    other items
                 *   
                 * What should happen:
                 *  + The stack should be increased to allow space to contain:
                 *         + .ctor arguments
                 *         + struct _pointer_ (ref to start of emptied space)
                 *         + empty space for struct
                 *  + arguments should be copied to the new place
                 *  + old place where arguments were should be cleared
                 *  + pointer should be set
                 *  + call .ctor
                 */
			    var xStorageSize = xTypeInfo.StorageSize;
                if (xStorageSize % 4 != 0) {
                    xStorageSize += 4 - (xStorageSize % 4);
                }
			    int xArgSize = (from item in xCtorInfo.Arguments
			                    select item.Size + (item.Size % 4 == 0
			                                            ? 0
			                                            : (4 - (item.Size % 4)))).Take(xCtorInfo.Arguments.Length - 1).Sum();
			    int xExtraArgSize = xStorageSize - xArgSize;
                if (xExtraArgSize < 0) {
                    xExtraArgSize = 0;
                }
                if(xExtraArgSize>0) {
                    new CPUx86.Sub("esp",
                                   xExtraArgSize.ToString());
                }
			    new CPUx86.Push("esp");
                aAssembler.StackContents.Push(new StackContent(4));
                //at this point, we need to move copy all arguments over. 
                for(int i = 0; i<(xArgSize/4);i++) {
                    new CPUx86.Pushd("[esp + " + (xStorageSize + 4) + "]"); // + 4 because the ptr is pushed too
                    new CPUx86.Move("dword [esp + " + (xStorageSize + 4 + 4) + "]",
                                    "0"); // clear the original place of the args
                }
			    var xCall = new Call(aCtorDef,
			                         aCurrentILOffset,
			                         true,
			                         aNextLabel);
                xCall.Assembler = aAssembler;
			    xCall.Assemble();
                aAssembler.StackContents.Push(new StackContent(xStorageSize,
                                                               aCtorDef.DeclaringType));
			}
		}
	}
}