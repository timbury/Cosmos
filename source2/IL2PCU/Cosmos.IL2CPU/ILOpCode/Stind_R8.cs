using System;

namespace Cosmos.IL2CPU.ILOpCodes
{
	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Stind_R8)]
	public class Stind_R8: ILOpCode
	{



		#region Old code
		// using System;
		// using System.IO;
		// 
		// 
		// using CPU = Indy.IL2CPU.Assembler.X86;
		// 
		// namespace Indy.IL2CPU.IL.X86 {
		// 	[Cosmos.IL2CPU.OpCode(ILOpCode.Code.Stind_R8)]
		// 	public class Stind_R8: ILOpCode {
		// 		public Stind_R8(ILReader aReader, MethodInformation aMethodInfo)
		// 			: base(aReader, aMethodInfo) {
		// 		}
		// 		public override void DoAssemble() {
		// 			Stind_I.Assemble(Assembler, 8);
		// 		}
		// 	}
		// }
		#endregion
	}
}
