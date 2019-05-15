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
namespace org.camunda.bpm.engine.test.util
{
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertEquals;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.junit.Assert.assertNotNull;


	using MigratingActivityInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingActivityInstanceValidationReport;
	using MigratingProcessInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingProcessInstanceValidationReport;
	using MigratingTransitionInstanceValidationReport = org.camunda.bpm.engine.migration.MigratingTransitionInstanceValidationReport;
	using ProcessInstance = org.camunda.bpm.engine.runtime.ProcessInstance;
	using Matcher = org.hamcrest.Matcher;
	using Matchers = org.hamcrest.Matchers;
	using Assert = org.junit.Assert;

	public class MigratingProcessInstanceValidationReportAssert
	{

	  protected internal MigratingProcessInstanceValidationReport actual;

	  public MigratingProcessInstanceValidationReportAssert(MigratingProcessInstanceValidationReport report)
	  {
		this.actual = report;
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert NotNull
	  {
		  get
		  {
			assertNotNull("Expected report to be not null", actual);
    
			return this;
		  }
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert hasProcessInstance(ProcessInstance processInstance)
	  {
		return hasProcessInstanceId(processInstance.Id);
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert hasProcessInstanceId(string processInstanceId)
	  {
		NotNull;

		assertEquals("Expected report to be for process instance", processInstanceId, actual.ProcessInstanceId);

		return this;
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert hasFailures(params string[] expectedFailures)
	  {
		NotNull;

		IList<string> actualFailures = actual.Failures;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super String>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super String>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		ICollection<Matcher> matchers = new List<Matcher>();
		foreach (string expectedFailure in expectedFailures)
		{
		  matchers.Add(Matchers.containsString(expectedFailure));
		}

		Assert.assertThat("Expected failures:\n" + joinFailures(Arrays.asList(expectedFailures)) + "But found failures:\n" + joinFailures(actualFailures), actualFailures, Matchers.containsInAnyOrder(matchers));

		return this;
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert hasActivityInstanceFailures(string sourceScopeId, params string[] expectedFailures)
	  {
		NotNull;

		MigratingActivityInstanceValidationReport actualReport = null;
		foreach (MigratingActivityInstanceValidationReport instanceReport in actual.ActivityInstanceReports)
		{
		  if (sourceScopeId.Equals(instanceReport.SourceScopeId))
		  {
			actualReport = instanceReport;
			break;
		  }
		}

		assertNotNull("No validation report found for source scope: " + sourceScopeId, actualReport);

		assertFailures(sourceScopeId, Arrays.asList(expectedFailures), actualReport.Failures);

		return this;
	  }

	  public virtual MigratingProcessInstanceValidationReportAssert hasTransitionInstanceFailures(string sourceScopeId, params string[] expectedFailures)
	  {
		NotNull;

		MigratingTransitionInstanceValidationReport actualReport = null;
		foreach (MigratingTransitionInstanceValidationReport instanceReport in actual.TransitionInstanceReports)
		{
		  if (sourceScopeId.Equals(instanceReport.SourceScopeId))
		  {
			actualReport = instanceReport;
			break;
		  }
		}

		assertNotNull("No validation report found for source scope: " + sourceScopeId, actualReport);

		assertFailures(sourceScopeId, Arrays.asList(expectedFailures), actualReport.Failures);

		return this;
	  }

	  protected internal virtual void assertFailures(string sourceScopeId, IList<string> expectedFailures, IList<string> actualFailures)
	  {

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super String>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super String>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		ICollection<Matcher> matchers = new List<Matcher>();
		foreach (string expectedFailure in expectedFailures)
		{
		  matchers.Add(Matchers.containsString(expectedFailure));
		}

		Assert.assertThat("Expected failures for source scope: " + sourceScopeId + "\n" + joinFailures(expectedFailures) + "But found failures:\n" + joinFailures(actualFailures), actualFailures, Matchers.containsInAnyOrder(matchers));
	  }

	  public static MigratingProcessInstanceValidationReportAssert assertThat(MigratingProcessInstanceValidationReport report)
	  {
		return new MigratingProcessInstanceValidationReportAssert(report);
	  }

	  public virtual string joinFailures(IList<string> failures)
	  {
		StringBuilder builder = new StringBuilder();
		foreach (object failure in failures)
		{
		  builder.Append("\t\t").Append(failure).Append("\n");
		}

		return builder.ToString();
	  }



	}

}