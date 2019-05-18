﻿using System.Collections.Generic;

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
namespace org.camunda.bpm.model.bpmn.instance
{
	using DataOutputRefs = org.camunda.bpm.model.bpmn.impl.instance.DataOutputRefs;
	using InputSetRefs = org.camunda.bpm.model.bpmn.impl.instance.InputSetRefs;
	using OptionalOutputRefs = org.camunda.bpm.model.bpmn.impl.instance.OptionalOutputRefs;
	using WhileExecutingOutputRefs = org.camunda.bpm.model.bpmn.impl.instance.WhileExecutingOutputRefs;


	/// <summary>
	/// @author Sebastian Menski
	/// </summary>
	public class OutputSetTest : BpmnModelElementInstanceTest
	{

	  public virtual TypeAssumption TypeAssumption
	  {
		  get
		  {
			return new TypeAssumption(typeof(BaseElement), false);
		  }
	  }

	  public virtual ICollection<ChildElementAssumption> ChildElementAssumptions
	  {
		  get
		  {
			return Arrays.asList(new ChildElementAssumption(typeof(DataOutputRefs)), new ChildElementAssumption(typeof(OptionalOutputRefs)), new ChildElementAssumption(typeof(WhileExecutingOutputRefs)), new ChildElementAssumption(typeof(InputSetRefs))
		   );
		  }
	  }

	  public virtual ICollection<AttributeAssumption> AttributesAssumptions
	  {
		  get
		  {
			return Arrays.asList(new AttributeAssumption("name")
		   );
		  }
	  }
	}

}