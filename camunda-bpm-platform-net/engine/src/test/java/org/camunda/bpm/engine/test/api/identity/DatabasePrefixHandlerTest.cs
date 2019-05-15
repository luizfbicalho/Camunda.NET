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
namespace org.camunda.bpm.engine.test.api.identity
{
	using DatabasePrefixHandler = org.camunda.bpm.engine.impl.digest.DatabasePrefixHandler;
	using Before = org.junit.Before;
	using Test = org.junit.Test;

//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.@is;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.CoreMatchers.nullValue;
//JAVA TO C# CONVERTER TODO TASK: This Java 'import static' statement cannot be converted to C#:
//	import static org.hamcrest.MatcherAssert.assertThat;

	public class DatabasePrefixHandlerTest
	{

	  internal DatabasePrefixHandler prefixHandler;

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Before public void inti()
	  public virtual void inti()
	  {
		prefixHandler = new DatabasePrefixHandler();
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testGeneratePrefix()
	  public virtual void testGeneratePrefix()
	  {

		// given
		string algorithmName = "test";

		// when
		string prefix = prefixHandler.generatePrefix(algorithmName);

		// then
		assertThat(prefix, @is("{test}"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void testRetrieveAlgorithmName()
	  public virtual void testRetrieveAlgorithmName()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.retrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		assertThat(algorithmName, @is("SHA"));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void retrieveAlgorithmForInvalidInput()
	  public virtual void retrieveAlgorithmForInvalidInput()
	  {

		// given
		string encryptedPasswordWithPrefix = "xxx{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.retrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		assertThat(algorithmName, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void retrieveAlgorithmWithMissingAlgorithmPrefix()
	  public virtual void retrieveAlgorithmWithMissingAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.retrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		assertThat(algorithmName, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void retrieveAlgorithmWithErroneousAlgorithmPrefix()
	  public virtual void retrieveAlgorithmWithErroneousAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHAn3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string algorithmName = prefixHandler.retrieveAlgorithmName(encryptedPasswordWithPrefix);

		// then
		assertThat(algorithmName, @is(nullValue()));
	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removePrefix()
	  public virtual void removePrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.removePrefix(encryptedPasswordWithPrefix);

		// then
		assertThat(encryptedPassword, @is("n3fE9/7XOmgD3BkeJlC+JLyb/Qg="));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removePrefixForInvalidInput()
	  public virtual void removePrefixForInvalidInput()
	  {

		// given
		string encryptedPasswordWithPrefix = "xxx{SHA}n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.removePrefix(encryptedPasswordWithPrefix);

		// then
		assertThat(encryptedPassword, @is(nullValue()));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removePrefixWithMissingAlgorithmPrefix()
	  public virtual void removePrefixWithMissingAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "n3fE9/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.removePrefix(encryptedPasswordWithPrefix);

		// then
		assertThat(encryptedPassword, @is(nullValue()));

	  }

//JAVA TO C# CONVERTER TODO TASK: Most Java annotations will not have direct .NET equivalent attributes:
//ORIGINAL LINE: @Test public void removePrefixWithErroneousAlgorithmPrefix()
	  public virtual void removePrefixWithErroneousAlgorithmPrefix()
	  {

		// given
		string encryptedPasswordWithPrefix = "SHAn3fE9}/7XOmgD3BkeJlC+JLyb/Qg=";

		// when
		string encryptedPassword = prefixHandler.removePrefix(encryptedPasswordWithPrefix);

		// then
		assertThat(encryptedPassword, @is(nullValue()));
	  }


	}

}