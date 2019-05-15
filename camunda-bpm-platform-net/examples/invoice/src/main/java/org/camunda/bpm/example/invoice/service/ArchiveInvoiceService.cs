/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.example.invoice.service
{

	using ProcessEngineException = org.camunda.bpm.engine.ProcessEngineException;
	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;

	/// <summary>
	/// <para>This is an empty service implementation illustrating how to use a plain
	/// Java Class as a BPMN 2.0 Service Task delegate.</para>
	/// </summary>
	public class ArchiveInvoiceService : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  private readonly Logger LOGGER = Logger.getLogger(typeof(ArchiveInvoiceService).FullName);

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {

		bool? shouldFail = (bool?) execution.getVariable("shouldFail");
		FileValue invoiceDocumentVar = execution.getVariableTyped("invoiceDocument");

		if (shouldFail != null && shouldFail)
		{
		  throw new ProcessEngineException("Could not archive invoice...");
		}
		else
		{
		  LOGGER.info("\n\n  ... Now archiving invoice " + execution.getVariable("invoiceNumber") + ", filename: " + invoiceDocumentVar.Filename + " \n\n");
		}

	  }

	}

}