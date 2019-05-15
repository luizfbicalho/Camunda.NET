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
namespace org.camunda.bpm.engine.test.api.authorization.util
{

	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public class AuthorizationScenario
	{

	  protected internal const string INDENTATION = "   ";

	  protected internal AuthorizationSpec[] givenAuthorizations = new AuthorizationSpec[]{};
	  protected internal AuthorizationSpec[] missingAuthorizations = new AuthorizationSpec[]{};

	  public static AuthorizationScenario scenario()
	  {
		return new AuthorizationScenario();
	  }

	  public virtual AuthorizationScenario withoutAuthorizations()
	  {
		return this;
	  }

	  public virtual AuthorizationScenario withAuthorizations(params AuthorizationSpec[] givenAuthorizations)
	  {
		this.givenAuthorizations = givenAuthorizations;
		return this;
	  }

	  public virtual AuthorizationScenario succeeds()
	  {
		return this;
	  }

	  public virtual AuthorizationScenario failsDueToRequired(params AuthorizationSpec[] expectedMissingAuthorizations)
	  {
		this.missingAuthorizations = expectedMissingAuthorizations;
		return this;
	  }

	  public virtual AuthorizationSpec[] GivenAuthorizations
	  {
		  get
		  {
			return givenAuthorizations;
		  }
	  }

	  public virtual AuthorizationSpec[] MissingAuthorizations
	  {
		  get
		  {
			return missingAuthorizations;
		  }
	  }

	  public override string ToString()
	  {
		StringBuilder sb = new StringBuilder();
		sb.Append("Given Authorizations: \n");
		foreach (AuthorizationSpec spec in givenAuthorizations)
		{
		  sb.Append(INDENTATION);
		  sb.Append(spec);
		  sb.Append("\n");
		}

		sb.Append("Expected missing authorizations: \n");
		foreach (AuthorizationSpec spec in missingAuthorizations)
		{
		  sb.Append(INDENTATION);
		  sb.Append(spec);
		  sb.Append("\n");
		}

		return sb.ToString();
	  }


	}

}