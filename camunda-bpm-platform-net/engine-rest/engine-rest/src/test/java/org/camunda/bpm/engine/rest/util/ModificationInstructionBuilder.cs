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
namespace org.camunda.bpm.engine.rest.util
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class ModificationInstructionBuilder
	{

	  public ModificationInstructionBuilder(string type)
	  {
		this.type = type;
	  }

//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal IDictionary<string, object> variables_Conflict;

	  protected internal string type;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string transitionInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string ancestorActivityInstanceId_Conflict;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string transitionId_Conflict;
	  protected internal bool isFlagSet;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool cancelCurrentActiveActivityInstances_Conflict;

	  public static ModificationInstructionBuilder cancellation()
	  {
		return new ModificationInstructionBuilder("cancel");
	  }

	  public static ModificationInstructionBuilder startBefore()
	  {
		return new ModificationInstructionBuilder("startBeforeActivity");
	  }

	  public static ModificationInstructionBuilder startAfter()
	  {
		return new ModificationInstructionBuilder("startAfterActivity");
	  }

	  public static ModificationInstructionBuilder startTransition()
	  {
		return new ModificationInstructionBuilder("startTransition");
	  }

	  public virtual ModificationInstructionBuilder variables(IDictionary<string, object> variables)
	  {
		this.variables_Conflict = variables;
		return this;
	  }

	  public virtual ModificationInstructionBuilder activityId(string activityId)
	  {
		this.activityId_Conflict = activityId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Conflict = activityInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder transitionInstanceId(string transitionInstanceId)
	  {
		this.transitionInstanceId_Conflict = transitionInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder ancestorActivityInstanceId(string ancestorActivityInstanceId)
	  {
		this.ancestorActivityInstanceId_Conflict = ancestorActivityInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder transitionId(string transitionId)
	  {
		this.transitionId_Conflict = transitionId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder cancelCurrentActiveActivityInstances(bool cancelCurrentActiveActivityInstances)
	  {
		isFlagSet = true;
		this.cancelCurrentActiveActivityInstances_Conflict = cancelCurrentActiveActivityInstances;
		return this;
	  }

	  public virtual IDictionary<string, object> Json
	  {
		  get
		  {
			IDictionary<string, object> json = new Dictionary<string, object>();
    
			json["type"] = type;
			json["activityId"] = activityId_Conflict;
			json["activityInstanceId"] = activityInstanceId_Conflict;
			json["transitionInstanceId"] = transitionInstanceId_Conflict;
			json["ancestorActivityInstanceId"] = ancestorActivityInstanceId_Conflict;
			json["variables"] = variables_Conflict;
			json["transitionId"] = transitionId_Conflict;
    
			if (type.Equals("cancel") && isFlagSet)
			{
			  json["cancelCurrentActiveActivityInstances"] = cancelCurrentActiveActivityInstances_Conflict;
			}
			return json;
		  }
	  }
	}

}