using System.Collections.Generic;
using System.Text;

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
namespace org.camunda.bpm.engine.impl.migration.validation.instance
{

	using MigratingActivityInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingActivityInstanceValidationReport;
	using MigratingProcessInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationReport;
	using MigratingTransitionInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingTransitionInstanceValidationReport;

	public class MigratingProcessInstanceValidationReportImpl : MigratingProcessInstanceValidationReport
	{

	  protected internal string processInstanceId;
	  protected internal IList<MigratingActivityInstanceValidationReport> activityInstanceReports = new List<MigratingActivityInstanceValidationReport>();
	  protected internal IList<MigratingTransitionInstanceValidationReport> transitionInstanceReports = new List<MigratingTransitionInstanceValidationReport>();
	  protected internal IList<string> failures = new List<string>();

	  public virtual string ProcessInstanceId
	  {
		  get
		  {
			return processInstanceId;
		  }
		  set
		  {
			this.processInstanceId = value;
		  }
	  }


	  public virtual void addActivityInstanceReport(MigratingActivityInstanceValidationReport instanceReport)
	  {
		activityInstanceReports.Add(instanceReport);
	  }

	  public virtual void addTransitionInstanceReport(MigratingTransitionInstanceValidationReport instanceReport)
	  {
		transitionInstanceReports.Add(instanceReport);
	  }

	  public virtual IList<MigratingActivityInstanceValidationReport> ActivityInstanceReports
	  {
		  get
		  {
			return activityInstanceReports;
		  }
	  }

	  public virtual IList<MigratingTransitionInstanceValidationReport> TransitionInstanceReports
	  {
		  get
		  {
			return transitionInstanceReports;
		  }
	  }

	  public virtual void addFailure(string failure)
	  {
		failures.Add(failure);
	  }

	  public virtual IList<string> Failures
	  {
		  get
		  {
			return failures;
		  }
	  }

	  public virtual bool hasFailures()
	  {
		return failures.Count > 0 || activityInstanceReports.Count > 0 || transitionInstanceReports.Count > 0;
	  }

	  public virtual void writeTo(StringBuilder sb)
	  {
		sb.Append("Cannot migrate process instance '").Append(processInstanceId).Append("':\n");

		foreach (string failure in failures)
		{
		  sb.Append("\t").Append(failure).Append("\n");
		}

		foreach (MigratingActivityInstanceValidationReport report in activityInstanceReports)
		{
		  sb.Append("\tCannot migrate activity instance '").Append(report.ActivityInstanceId).Append("':\n");

		  foreach (string failure in report.Failures)
		  {
			sb.Append("\t\t").Append(failure).Append("\n");
		  }
		}

		foreach (MigratingTransitionInstanceValidationReport report in transitionInstanceReports)
		{
		  sb.Append("\tCannot migrate transition instance '").Append(report.TransitionInstanceId).Append("':\n");

		  foreach (string failure in report.Failures)
		  {
			sb.Append("\t\t").Append(failure).Append("\n");
		  }
		}
	  }

	}

}