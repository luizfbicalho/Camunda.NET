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
	/// Allows to follows the progress of a <seealso cref="PerfTestRun"/>.
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public interface PerfTestWatcher
	{

	  /// <summary>
	  /// Invoked before a performance test pass is started.
	  /// </summary>
	  /// <param name="pass"> the current <seealso cref="PerfTestPass"/> </param>
	  void beforePass(PerfTestPass pass);

	  /// <summary>
	  /// Invoked before a performance test run is started.
	  /// </summary>
	  /// <param name="test"> the <seealso cref="PerfTest"/> about to be executed </param>
	  /// <param name="run"> the current <seealso cref="PerfTestRun"/> </param>
	  void beforeRun(PerfTest test, PerfTestRun run);

	  /// <summary>
	  /// Invoked before a <seealso cref="PerfTestRun"/> starts an individual
	  /// step in the performance test.
	  /// 
	  /// This method is called by the same <seealso cref="Thread"/> which will
	  /// execute the step.
	  /// </summary>
	  /// <param name="step"> the <seealso cref="PerfTestStep"/> about to be executed. </param>
	  /// <param name="run"> the current <seealso cref="PerfTestRun"/> </param>
	  void beforeStep(PerfTestStep step, PerfTestRun run);

	  /// <summary>
	  /// Invoked after a <seealso cref="PerfTestRun"/> ends an individual
	  /// step in the performance test.
	  /// 
	  /// This method is called by the same <seealso cref="Thread"/> which
	  /// executed the step.
	  /// </summary>
	  /// <param name="step"> the <seealso cref="PerfTestStep"/> which has been executed. </param>
	  /// <param name="run"> the current <seealso cref="PerfTestRun"/> </param>
	  void afterStep(PerfTestStep step, PerfTestRun run);

	  /// <summary>
	  /// Invoked after a performance test run is ended.
	  /// </summary>
	  /// <param name="test"> the <seealso cref="PerfTest"/> about to be executed </param>
	  /// <param name="run"> the current <seealso cref="PerfTestRun"/> </param>
	  void afterRun(PerfTest test, PerfTestRun run);

	  /// <summary>
	  /// Invoked after a performance test pass is ended.
	  /// </summary>
	  /// <param name="pass"> the current <seealso cref="PerfTestPass"/> </param>
	  void afterPass(PerfTestPass pass);
	}

}