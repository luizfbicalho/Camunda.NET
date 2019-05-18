using System.Collections.Generic;

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
namespace org.camunda.bpm.model.bpmn.validation
{

	using ModelElementInstance = org.camunda.bpm.model.xml.instance.ModelElementInstance;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResult = org.camunda.bpm.model.xml.validation.ValidationResult;
	using ValidationResultType = org.camunda.bpm.model.xml.validation.ValidationResultType;
	using ValidationResults = org.camunda.bpm.model.xml.validation.ValidationResults;
	using Test = org.junit.Test;
	using Process = org.camunda.bpm.model.bpmn.instance.Process;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.*;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ValidateProcessTest
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void validationFailsIfNoStartEventFound()
	  public virtual void validationFailsIfNoStartEventFound()
	  {

//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: java.util.List<org.camunda.bpm.model.xml.validation.ModelElementValidator<?>> validators = new java.util.ArrayList<org.camunda.bpm.model.xml.validation.ModelElementValidator<?>>();
		IList<ModelElementValidator<object>> validators = new List<ModelElementValidator<object>>();
		validators.Add(new ProcessStartEventValidator());

		BpmnModelInstance bpmnModelInstance = Bpmn.createProcess().done();

		ValidationResults validationResults = bpmnModelInstance.validate(validators);

		assertThat(validationResults.hasErrors()).True;

		IDictionary<ModelElementInstance, IList<ValidationResult>> results = validationResults.Results;
		assertThat(results.Count).isEqualTo(1);

		Process process = bpmnModelInstance.Definitions.getChildElementsByType(typeof(Process)).GetEnumerator().next();
		assertThat(results.ContainsKey(process)).True;

		IList<ValidationResult> resultsForProcess = results[process];
		assertThat(resultsForProcess.Count).isEqualTo(1);

		ValidationResult validationResult = resultsForProcess[0];
		assertThat(validationResult.Element).isEqualTo(process);
		assertThat(validationResult.Code).isEqualTo(10);
		assertThat(validationResult.Message).isEqualTo("Process does not have exactly one start event. Got 0.");
		assertThat(validationResult.Type).isEqualTo(ValidationResultType.ERROR);

	  }

	}

}