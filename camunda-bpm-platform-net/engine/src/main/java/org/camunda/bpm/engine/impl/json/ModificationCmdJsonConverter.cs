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
namespace org.camunda.bpm.engine.impl.json
{
	using AbstractProcessInstanceModificationCommand = org.camunda.bpm.engine.impl.cmd.AbstractProcessInstanceModificationCommand;
	using ActivityAfterInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityAfterInstantiationCmd;
	using ActivityBeforeInstantiationCmd = org.camunda.bpm.engine.impl.cmd.ActivityBeforeInstantiationCmd;
	using ActivityCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityCancellationCmd;
	using ActivityInstanceCancellationCmd = org.camunda.bpm.engine.impl.cmd.ActivityInstanceCancellationCmd;
	using TransitionInstanceCancellationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstanceCancellationCmd;
	using TransitionInstantiationCmd = org.camunda.bpm.engine.impl.cmd.TransitionInstantiationCmd;
	using JsonUtil = org.camunda.bpm.engine.impl.util.JsonUtil;
	using JsonObject = com.google.gson.JsonObject;

	public class ModificationCmdJsonConverter : JsonObjectConverter<AbstractProcessInstanceModificationCommand>
	{

	  public static readonly ModificationCmdJsonConverter INSTANCE = new ModificationCmdJsonConverter();

	  public const string START_BEFORE = "startBeforeActivity";
	  public const string START_AFTER = "startAfterActivity";
	  public const string START_TRANSITION = "startTransition";
	  public const string CANCEL_ALL = "cancelAllForActivity";
	  public const string CANCEL_CURRENT = "cancelCurrentActiveActivityInstances";
	  public const string CANCEL_ACTIVITY_INSTANCES = "cancelActivityInstances";
	  public const string PROCESS_INSTANCE = "processInstances";
	  public const string CANCEL_TRANSITION_INSTANCES = "cancelTransitionInstances";

	  public override JsonObject toJsonObject(AbstractProcessInstanceModificationCommand command)
	  {
		JsonObject json = JsonUtil.createObject();

		if (command is ActivityAfterInstantiationCmd)
		{
		  JsonUtil.addField(json, START_AFTER, ((ActivityAfterInstantiationCmd) command).TargetElementId);
		}
		else if (command is ActivityBeforeInstantiationCmd)
		{
		  JsonUtil.addField(json, START_BEFORE, ((ActivityBeforeInstantiationCmd) command).TargetElementId);
		}
		else if (command is TransitionInstantiationCmd)
		{
		  JsonUtil.addField(json, START_TRANSITION, ((TransitionInstantiationCmd) command).TargetElementId);
		}
		else if (command is ActivityCancellationCmd)
		{
		  JsonUtil.addField(json, CANCEL_ALL, ((ActivityCancellationCmd) command).ActivityId);
		  JsonUtil.addField(json, CANCEL_CURRENT, ((ActivityCancellationCmd) command).CancelCurrentActiveActivityInstances);
		}
		else if (command is ActivityInstanceCancellationCmd)
		{
		  JsonUtil.addField(json, CANCEL_ACTIVITY_INSTANCES, ((ActivityInstanceCancellationCmd) command).ActivityInstanceId);
		  JsonUtil.addField(json, PROCESS_INSTANCE, ((ActivityInstanceCancellationCmd) command).ProcessInstanceId);
		}
		else if (command is TransitionInstanceCancellationCmd)
		{
		  JsonUtil.addField(json, CANCEL_TRANSITION_INSTANCES, ((TransitionInstanceCancellationCmd) command).TransitionInstanceId);
		  JsonUtil.addField(json, PROCESS_INSTANCE, ((TransitionInstanceCancellationCmd) command).ProcessInstanceId);
		}

		return json;
	  }

	  public override AbstractProcessInstanceModificationCommand toObject(JsonObject json)
	  {

		AbstractProcessInstanceModificationCommand cmd = null;

		if (json.has(START_BEFORE))
		{
		  cmd = new ActivityBeforeInstantiationCmd(JsonUtil.getString(json, START_BEFORE));
		}
		else if (json.has(START_AFTER))
		{
		  cmd = new ActivityAfterInstantiationCmd(JsonUtil.getString(json, START_AFTER));
		}
		else if (json.has(START_TRANSITION))
		{
		  cmd = new TransitionInstantiationCmd(JsonUtil.getString(json, START_TRANSITION));
		}
		else if (json.has(CANCEL_ALL))
		{
		  cmd = new ActivityCancellationCmd(JsonUtil.getString(json, CANCEL_ALL));
		  bool cancelCurrentActiveActivityInstances = JsonUtil.getBoolean(json, CANCEL_CURRENT);
		  ((ActivityCancellationCmd) cmd).CancelCurrentActiveActivityInstances = cancelCurrentActiveActivityInstances;
		}
		else if (json.has(CANCEL_ACTIVITY_INSTANCES))
		{
		  cmd = new ActivityInstanceCancellationCmd(JsonUtil.getString(json, PROCESS_INSTANCE), JsonUtil.getString(json, CANCEL_ACTIVITY_INSTANCES));
		}
		else if (json.has(CANCEL_TRANSITION_INSTANCES))
		{
		  cmd = new TransitionInstanceCancellationCmd(JsonUtil.getString(json, PROCESS_INSTANCE), JsonUtil.getString(json, CANCEL_TRANSITION_INSTANCES));
		}

		return cmd;
	  }

	}

}