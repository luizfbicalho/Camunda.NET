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
namespace org.camunda.commons.logging
{
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.assertj.core.api.Assertions.*;
	using static org.camunda.commons.logging.ExampleLogger;

	/// <summary>
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class BaseLoggerTests
	{

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFormatMessage()
	  public virtual void shouldFormatMessage()
	  {
		ExampleLogger logger = LOG;

		string id = "01";
		string messageTemplate = "Some message '{}'";

		string formattedMessage = logger.formatMessageTemplate(id, messageTemplate);
		string expectedMessageTemplate = string.Format("{0}-{1}{2} {3}", PROJECT_CODE, COMPONENT_ID, id, messageTemplate);

		assertThat(formattedMessage).isEqualTo(expectedMessageTemplate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFormatExceptionMessageWithParam()
	  public virtual void shouldFormatExceptionMessageWithParam()
	  {
		ExampleLogger logger = LOG;

		string id = "01";
		string messageTemplate = "Some message '{}'";
		string parameter = "someParameter";

		string formattedMessage = logger.exceptionMessage(id, messageTemplate, parameter);
		string expectedMessageTemplate = string.Format("{0}-{1}{2} Some message 'someParameter'", PROJECT_CODE, COMPONENT_ID, id);

		assertThat(formattedMessage).isEqualTo(expectedMessageTemplate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFormatExceptionMessageWithParams()
	  public virtual void shouldFormatExceptionMessageWithParams()
	  {
		ExampleLogger logger = LOG;

		string id = "01";
		string messageTemplate = "Some message '{}' '{}'";
		string p1 = "someParameter";
		string p2 = "someOtherParameter";

		string formattedMessage = logger.exceptionMessage(id, messageTemplate, p1, p2);
		string expectedMessageTemplate = string.Format("{0}-{1}{2} Some message 'someParameter' 'someOtherParameter'", PROJECT_CODE, COMPONENT_ID, id);

		assertThat(formattedMessage).isEqualTo(expectedMessageTemplate);
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void shouldFormatExceptionMessageWithoutParam()
	  public virtual void shouldFormatExceptionMessageWithoutParam()
	  {
		ExampleLogger logger = LOG;

		string id = "01";
		string messageTemplate = "Some message";

		string formattedMessage = logger.exceptionMessage(id, messageTemplate);
		string expectedMessageTemplate = string.Format("{0}-{1}{2} {3}", PROJECT_CODE, COMPONENT_ID, id, messageTemplate);

		assertThat(formattedMessage).isEqualTo(expectedMessageTemplate);
	  }


	}

}