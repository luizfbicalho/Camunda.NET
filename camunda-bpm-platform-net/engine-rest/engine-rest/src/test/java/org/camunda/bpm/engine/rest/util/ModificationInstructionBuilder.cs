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
	  protected internal IDictionary<string, object> variables_Renamed;

	  protected internal string type;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string activityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string transitionInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string ancestorActivityInstanceId_Renamed;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal string transitionId_Renamed;
	  protected internal bool isFlagSet;
//JAVA TO C# CONVERTER NOTE: Fields cannot have the same name as methods:
	  protected internal bool cancelCurrentActiveActivityInstances_Renamed;

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
		this.variables_Renamed = variables;
		return this;
	  }

	  public virtual ModificationInstructionBuilder activityId(string activityId)
	  {
		this.activityId_Renamed = activityId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder activityInstanceId(string activityInstanceId)
	  {
		this.activityInstanceId_Renamed = activityInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder transitionInstanceId(string transitionInstanceId)
	  {
		this.transitionInstanceId_Renamed = transitionInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder ancestorActivityInstanceId(string ancestorActivityInstanceId)
	  {
		this.ancestorActivityInstanceId_Renamed = ancestorActivityInstanceId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder transitionId(string transitionId)
	  {
		this.transitionId_Renamed = transitionId;
		return this;
	  }

	  public virtual ModificationInstructionBuilder cancelCurrentActiveActivityInstances(bool cancelCurrentActiveActivityInstances)
	  {
		isFlagSet = true;
		this.cancelCurrentActiveActivityInstances_Renamed = cancelCurrentActiveActivityInstances;
		return this;
	  }

	  public virtual IDictionary<string, object> Json
	  {
		  get
		  {
			IDictionary<string, object> json = new Dictionary<string, object>();
    
			json["type"] = type;
			json["activityId"] = activityId_Renamed;
			json["activityInstanceId"] = activityInstanceId_Renamed;
			json["transitionInstanceId"] = transitionInstanceId_Renamed;
			json["ancestorActivityInstanceId"] = ancestorActivityInstanceId_Renamed;
			json["variables"] = variables_Renamed;
			json["transitionId"] = transitionId_Renamed;
    
			if (type.Equals("cancel") && isFlagSet)
			{
			  json["cancelCurrentActiveActivityInstances"] = cancelCurrentActiveActivityInstances_Renamed;
			}
			return json;
		  }
	  }
	}

}