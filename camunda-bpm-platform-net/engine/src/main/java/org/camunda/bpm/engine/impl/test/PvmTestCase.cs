using System;

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
namespace org.camunda.bpm.engine.impl.test
{
	using AssertionFailedError = junit.framework.AssertionFailedError;
	using TestCase = junit.framework.TestCase;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using CaseControlRuleImpl = org.camunda.bpm.engine.impl.cmmn.behavior.CaseControlRuleImpl;
	using FixedValue = org.camunda.bpm.engine.impl.el.FixedValue;


	/// <summary>
	/// @author Tom Baeyens
	/// </summary>
	public class PvmTestCase : TestCase
	{

	  /// <summary>
	  /// Asserts if the provided text is part of some text.
	  /// </summary>
	  public virtual void assertTextPresent(string expected, string actual)
	  {
		if ((string.ReferenceEquals(actual, null)) || (actual.IndexOf(expected, StringComparison.Ordinal) == -1))
		{
		  throw new AssertionFailedError("expected presence of [" + expected + "], but was [" + actual + "]");
		}
	  }

	  /// <summary>
	  /// Asserts if the provided text is part of some text, ignoring any uppercase characters
	  /// </summary>
	  public virtual void assertTextPresentIgnoreCase(string expected, string actual)
	  {
		assertTextPresent(expected.ToLower(), actual.ToLower());
	  }

	  public virtual object defaultManualActivation()
	  {
		Expression expression = new FixedValue(true);
		CaseControlRuleImpl caseControlRule = new CaseControlRuleImpl(expression);
		return caseControlRule;
	  }

	}

}