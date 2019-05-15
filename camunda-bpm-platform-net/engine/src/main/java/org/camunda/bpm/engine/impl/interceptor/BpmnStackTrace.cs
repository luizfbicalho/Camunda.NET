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
namespace org.camunda.bpm.engine.impl.interceptor
{

	/// 
	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BpmnStackTrace
	{

	  private static readonly ContextLogger LOG = ProcessEngineLogger.CONTEXT_LOGGER;

	  protected internal IList<AtomicOperationInvocation> perfromedInvocations = new List<AtomicOperationInvocation>();

	  public virtual void printStackTrace(bool verbose)
	  {
		if (perfromedInvocations.Count == 0)
		{
		  return;
		}

		StringWriter writer = new StringWriter();
		writer.write("BPMN Stack Trace:\n");

		if (!verbose)
		{
		  logNonVerbose(writer);
		}
		else
		{
		  logVerbose(writer);
		}

		LOG.bpmnStackTrace(writer.ToString());

		perfromedInvocations.Clear();
	  }

	  protected internal virtual void logNonVerbose(StringWriter writer)
	  {

		// log the failed operation verbosely
		writeInvocation(perfromedInvocations[perfromedInvocations.Count - 1], writer);

		// log human consumable trace of activity ids and names
		IList<IDictionary<string, string>> activityTrace = collectActivityTrace();
		logActivityTrace(writer, activityTrace);
	  }

	  protected internal virtual void logVerbose(StringWriter writer)
	  {
		// log process engine developer consumable trace
		perfromedInvocations.Reverse();
		foreach (AtomicOperationInvocation invocation in perfromedInvocations)
		{
		  writeInvocation(invocation, writer);
		}
	  }

	  protected internal virtual void logActivityTrace(StringWriter writer, IList<IDictionary<string, string>> activities)
	  {
		for (int i = 0; i < activities.Count; i++)
		{
		  if (i != 0)
		  {
			writer.write("\t  ^\n");
			writer.write("\t  |\n");
		  }
		  writer.write("\t");

		  IDictionary<string, string> activity = activities[i];
		  string activityId = activity["activityId"];
		  writer.write(activityId);

		  string activityName = activity["activityName"];
		  if (!string.ReferenceEquals(activityName, null))
		  {
			writer.write(", name=");
			writer.write(activityName);
		  }

		  writer.write("\n");
		}
	  }

	  protected internal virtual IList<IDictionary<string, string>> collectActivityTrace()
	  {
		IList<IDictionary<string, string>> activityTrace = new List<IDictionary<string, string>>();
		foreach (AtomicOperationInvocation atomicOperationInvocation in perfromedInvocations)
		{
		  string activityId = atomicOperationInvocation.ActivityId;
		  if (string.ReferenceEquals(activityId, null))
		  {
			continue;
		  }

		  IDictionary<string, string> activity = new Dictionary<string, string>();
		  activity["activityId"] = activityId;

		  string activityName = atomicOperationInvocation.ActivityName;
		  if (!string.ReferenceEquals(activityName, null))
		  {
			activity["activityName"] = activityName;
		  }

		  if (activityTrace.Count == 0 || !activity["activityId"].Equals(activityTrace[0]["activityId"]))
		  {
			activityTrace.Insert(0, activity);
		  }
		}
		return activityTrace;
	  }

	  public virtual void add(AtomicOperationInvocation atomicOperationInvocation)
	  {
		perfromedInvocations.Add(atomicOperationInvocation);
	  }

	  protected internal virtual void writeInvocation(AtomicOperationInvocation invocation, StringWriter writer)
	  {
		writer.write("\t");
		writer.write(invocation.ActivityId);
		writer.write(" (");
		writer.write(invocation.Operation.CanonicalName);
		writer.write(", ");
		writer.write(invocation.Execution.ToString());

		if (invocation.PerformAsync)
		{
		  writer.write(", ASYNC");
		}

		if (!string.ReferenceEquals(invocation.ApplicationContextName, null))
		{
		  writer.write(", pa=");
		  writer.write(invocation.ApplicationContextName);
		}

		writer.write(")\n");
	  }

	}

}