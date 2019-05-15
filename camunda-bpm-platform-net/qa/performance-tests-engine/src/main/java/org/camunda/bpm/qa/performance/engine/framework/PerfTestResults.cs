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
namespace org.camunda.bpm.qa.performance.engine.framework
{

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class PerfTestResults
	{

	  /// <summary>
	  /// the name of the test </summary>
	  protected internal string testName;

	  /// <summary>
	  /// the configuration used </summary>
	  protected internal PerfTestConfiguration configuration;

	  /// <summary>
	  /// the individual result entries * </summary>
	  protected internal IList<PerfTestResult> passResults = new List<PerfTestResult>();

	  public PerfTestResults(PerfTestConfiguration configuration)
	  {
		this.configuration = configuration;
	  }

	  public PerfTestResults()
	  {
	  }

	  // getter / setters ////////////////////////////

	  public virtual string TestName
	  {
		  get
		  {
			return testName;
		  }
		  set
		  {
			this.testName = value;
		  }
	  }


	  public virtual PerfTestConfiguration Configuration
	  {
		  get
		  {
			return configuration;
		  }
		  set
		  {
			this.configuration = value;
		  }
	  }


	  public virtual IList<PerfTestResult> PassResults
	  {
		  get
		  {
			return passResults;
		  }
		  set
		  {
			this.passResults = value;
		  }
	  }


	}

}