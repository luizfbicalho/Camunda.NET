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
namespace org.camunda.bpm.engine.impl.migration.validation.instruction
{

	using CallActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CallActivityBehavior;
	using CaseCallActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.CaseCallActivityBehavior;
	using EventSubProcessActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.EventSubProcessActivityBehavior;
	using SubProcessActivityBehavior = org.camunda.bpm.engine.impl.bpmn.behavior.SubProcessActivityBehavior;
	using ActivityImpl = org.camunda.bpm.engine.impl.pvm.process.ActivityImpl;
	using CollectionUtil = org.camunda.bpm.engine.impl.util.CollectionUtil;

	public class SameBehaviorInstructionValidator : MigrationInstructionValidator
	{

	  public static readonly IList<ISet<Type>> EQUIVALENT_BEHAVIORS = new List<ISet<Type>>();

	  static SameBehaviorInstructionValidator()
	  {
		EQUIVALENT_BEHAVIORS.Add(CollectionUtil.asHashSet<Type>(typeof(CallActivityBehavior), typeof(CaseCallActivityBehavior)));

		EQUIVALENT_BEHAVIORS.Add(CollectionUtil.asHashSet<Type>(typeof(SubProcessActivityBehavior), typeof(EventSubProcessActivityBehavior)));
	  }

	  protected internal IDictionary<Type, ISet<Type>> equivalentBehaviors = new Dictionary<Type, ISet<Type>>();

	  public SameBehaviorInstructionValidator() : this(EQUIVALENT_BEHAVIORS)
	  {
	  }

	  public SameBehaviorInstructionValidator(IList<ISet<Type>> equivalentBehaviors)
	  {
		foreach (ISet<Type> equivalenceClass in equivalentBehaviors)
		{
		  foreach (Type clazz in equivalenceClass)
		  {
			this.equivalentBehaviors[clazz] = equivalenceClass;
		  }
		}
	  }

	  public virtual void validate(ValidatingMigrationInstruction instruction, ValidatingMigrationInstructions instructions, MigrationInstructionValidationReportImpl report)
	  {
		ActivityImpl sourceActivity = instruction.SourceActivity;
		ActivityImpl targetActivity = instruction.TargetActivity;

		Type sourceBehaviorClass = sourceActivity.ActivityBehavior.GetType();
		Type targetBehaviorClass = targetActivity.ActivityBehavior.GetType();

		if (!sameBehavior(sourceBehaviorClass, targetBehaviorClass))
		{
		  report.addFailure("Activities have incompatible types " + "(" + sourceBehaviorClass.Name + " is not compatible with " + targetBehaviorClass.Name + ")");
		}
	  }

	  protected internal virtual bool sameBehavior(Type sourceBehavior, Type targetBehavior)
	  {

		if (sourceBehavior == targetBehavior)
		{
		  return true;
		}
		else
		{
		  ISet<Type> equivalentBehaviors = this.equivalentBehaviors[sourceBehavior];
		  if (equivalentBehaviors != null)
		  {
			return equivalentBehaviors.Contains(targetBehavior);
		  }
		  else
		  {
			return false;
		  }
		}
	  }

	}

}