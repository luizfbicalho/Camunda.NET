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
//	import static org.junit.Assert.assertNotNull;


	using MigrationInstructionValidationReport = org.camunda.bpm.engine.migration.MigrationInstructionValidationReport;
	using MigrationPlanValidationReport = org.camunda.bpm.engine.migration.MigrationPlanValidationReport;
	using Matcher = org.hamcrest.Matcher;
	using Matchers = org.hamcrest.Matchers;
	using Assert = org.junit.Assert;

	public class MigrationPlanValidationReportAssert
	{

	  protected internal MigrationPlanValidationReport actual;

	  public MigrationPlanValidationReportAssert(MigrationPlanValidationReport report)
	  {
		this.actual = report;
	  }

	  public virtual MigrationPlanValidationReportAssert NotNull
	  {
		  get
		  {
			assertNotNull("Expected report to be not null", actual);
    
			return this;
		  }
	  }

	  public virtual MigrationPlanValidationReportAssert hasInstructionFailures(string activityId, params string[] expectedFailures)
	  {
		NotNull;

		IList<string> failuresFound = new List<string>();

		foreach (MigrationInstructionValidationReport instructionReport in actual.InstructionReports)
		{
		  string sourceActivityId = instructionReport.MigrationInstruction.SourceActivityId;
		  if ((string.ReferenceEquals(activityId, null) && string.ReferenceEquals(sourceActivityId, null)) || (!string.ReferenceEquals(activityId, null) && activityId.Equals(sourceActivityId)))
		  {
			((IList<string>)failuresFound).AddRange(instructionReport.Failures);
		  }
		}

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the Java 'super' constraint:
//ORIGINAL LINE: java.util.Collection<org.hamcrest.Matcher<? super String>> matchers = new java.util.ArrayList<org.hamcrest.Matcher<? super String>>();
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
		ICollection<Matcher> matchers = new List<Matcher>();
		foreach (string expectedFailure in expectedFailures)
		{
		  matchers.Add(Matchers.containsString(expectedFailure));
		}

		Assert.assertThat("Expected failures for activity id '" + activityId + "':\n" + joinFailures(expectedFailures) + "But found failures:\n" + joinFailures(failuresFound.ToArray()), failuresFound, Matchers.containsInAnyOrder(matchers));

		return this;
	  }

	  public static MigrationPlanValidationReportAssert assertThat(MigrationPlanValidationReport report)
	  {
		return new MigrationPlanValidationReportAssert(report);
	  }

	  public virtual string joinFailures(object[] failures)
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