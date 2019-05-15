using System;
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
namespace org.camunda.bpm.engine.rest.helper
{
	using Reporter = org.mockito.exceptions.Reporter;
	using MockitoAssertionError = org.mockito.exceptions.@base.MockitoAssertionError;
	using InvocationsFinder = org.mockito.@internal.invocation.InvocationsFinder;
	using VerificationData = org.mockito.@internal.verification.api.VerificationData;
	using VerificationDataInOrder = org.mockito.@internal.verification.api.VerificationDataInOrder;
	using VerificationInOrderMode = org.mockito.@internal.verification.api.VerificationInOrderMode;
	using Invocation = org.mockito.invocation.Invocation;
	using VerificationMode = org.mockito.verification.VerificationMode;

	/// <summary>
	/// Stricter verification mode than regular Mockito inOrder verification by requiring
	/// the given invocation to occur directly after the last verified invocation. May be
	/// useful to verify APIs where order is important, e.g. fluent builders.
	/// 
	/// @author Thorben Lindhauer
	/// </summary>
	public class NoIntermediaryInvocation : VerificationInOrderMode, VerificationMode
	{

	  protected internal InvocationsFinder finder = new InvocationsFinder();

	  public override void verifyInOrder(VerificationDataInOrder data)
	  {

		Invocation firstUnverifiedInvocation = finder.findFirstUnverifiedInOrder(data.OrderingContext, data.AllInvocations);

		if (firstUnverifiedInvocation == null)
		{
		  Invocation previouslyVerified = finder.findPreviousVerifiedInOrder(data.AllInvocations, data.OrderingContext);
		  (new Reporter()).wantedButNotInvokedInOrder(data.Wanted, previouslyVerified);
		}

		if (!data.Wanted.matches(firstUnverifiedInvocation))
		{
		  StringBuilder sb = new StringBuilder();
		  sb.Append("Expected next invocation specified here: \n");
		  sb.Append(data.Wanted.Location);
		  sb.Append("\n");
		  sb.Append("but next invocation was: \n");
		  sb.Append(firstUnverifiedInvocation.Location);
		  sb.Append("\n");

		  throw new MockitoAssertionError(sb.ToString());
		}
	  }

	  public override void verify(VerificationData data)
	  {
		throw new Exception("Applies only to inorder verification");
	  }

	  public static NoIntermediaryInvocation immediatelyAfter()
	  {
		return new NoIntermediaryInvocation();
	  }

	}

}