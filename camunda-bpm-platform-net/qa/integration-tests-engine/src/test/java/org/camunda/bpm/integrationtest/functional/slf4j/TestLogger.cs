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
namespace org.camunda.bpm.integrationtest.functional.slf4j
{
	using BaseLogger = org.camunda.commons.logging.BaseLogger;
	using MessageFormatter = org.slf4j.helpers.MessageFormatter;

	public class TestLogger : BaseLogger
	{

	  public static readonly TestLogger INSTANCE = BaseLogger.createLogger(typeof(TestLogger), "QA", "org.camunda.bpm.qa", "01");

	  /// <summary>
	  /// Verify that camunda commons log a messages with a single format parameter.
	  /// The return type of <seealso cref="MessageFormatter.format(string, object)"/> changed with slf4j-api:1.6.0
	  /// </summary>
	  public virtual void testLogWithSingleFormatParameter()
	  {
		logInfo("001", "This is a test of the SLF4J array formatter return type: {}", "Test test");
	  }

	  /// <summary>
	  /// Verify that camunda commons log a messages with two format parameters.
	  /// The return type of <seealso cref="MessageFormatter.format(string, object, object)"/> changed with slf4j-api:1.6.0
	  /// </summary>
	  public virtual void testLogWithTwoFormatParameters()
	  {
		logInfo("002", "This is a test of the SLF4J array formatter return type: {}, {}", "Test test", 123);
	  }

	  /// <summary>
	  /// Verify that camunda commons log a messages which uses the array formatter (more than two format parameters).
	  /// The return type of <seealso cref="MessageFormatter.arrayFormat(string, Object[])"/> changed with slf4j-api:1.6.0
	  /// </summary>
	  public virtual void testLogWithArrayFormatter()
	  {
		// we must used at least 3 parameters to reach the array formatter
		logInfo("003", "This is a test of the SLF4J array formatter return type: {}, {}, {}, {}", "Test test", 123, true, "it seems to work so slf4j >= 1.6.0 is used");
	  }

	}

}