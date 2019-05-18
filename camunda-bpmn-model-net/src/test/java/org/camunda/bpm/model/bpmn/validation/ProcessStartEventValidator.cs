using System;
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

	using Process = org.camunda.bpm.model.bpmn.instance.Process;
	using StartEvent = org.camunda.bpm.model.bpmn.instance.StartEvent;
	using ModelElementValidator = org.camunda.bpm.model.xml.validation.ModelElementValidator;
	using ValidationResultCollector = org.camunda.bpm.model.xml.validation.ValidationResultCollector;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ProcessStartEventValidator : ModelElementValidator<Process>
	{

	  public override Type<Process> ElementType
	  {
		  get
		  {
			return typeof(Process);
		  }
	  }

	  public override void validate(Process process, ValidationResultCollector validationResultCollector)
	  {
		ICollection<StartEvent> startEvents = process.getChildElementsByType(typeof(StartEvent));
		int startEventCount = startEvents.Count;

		if (startEventCount != 1)
		{
		  validationResultCollector.addError(10, string.Format("Process does not have exactly one start event. Got {0:D}.", startEventCount));
		}
	  }

	}

}