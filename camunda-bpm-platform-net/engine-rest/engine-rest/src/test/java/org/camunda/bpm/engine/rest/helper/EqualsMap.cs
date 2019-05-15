﻿using System.Collections.Generic;

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

	using VariableMap = org.camunda.bpm.engine.variable.VariableMap;
	using Matcher = org.hamcrest.Matcher;
	using ArgumentMatcher = org.mockito.ArgumentMatcher;

	public class EqualsMap : ArgumentMatcher<IDictionary<string, object>>
	{

	  protected internal IDictionary<string, object> mapToCompare;
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: protected java.util.Map<String, org.hamcrest.Matcher<?>> matchers;
	  protected internal IDictionary<string, Matcher<object>> matchers;

	  public EqualsMap()
	  {
	  }

	  public EqualsMap(IDictionary<string, object> mapToCompare)
	  {
		this.mapToCompare = mapToCompare;
	  }

	  public override bool matches(object argument)
	  {
		if (mapToCompare != null)
		{
		  return matchesExactly(argument);
		}
		else if (matchers != null)
		{
		  return matchesMatchers(argument);
		}
		else
		{
		  return argument == null;
		}
	  }

	  protected internal virtual bool matchesExactly(object argument)
	  {
		if (argument == null)
		{
		  return false;
		}

		IDictionary<string, object> argumentMap = (IDictionary<string, object>) argument;

		ISet<KeyValuePair<string, object>> setToCompare = mapToCompare.SetOfKeyValuePairs();
		ISet<KeyValuePair<string, object>> argumentSet = argumentMap.SetOfKeyValuePairs();

		return setToCompare.SetEquals(argumentSet);
	  }

	  protected internal virtual bool matchesMatchers(object argument)
	  {
		if (argument == null)
		{
		  return false;
		}

		IDictionary<string, object> argumentMap = (IDictionary<string, object>) argument;

//JAVA TO C# CONVERTER TODO TASK: There is no .NET equivalent to the java.util.Collection 'containsAll' method:
		bool containSameKeys = matchers.Keys.containsAll(argumentMap.Keys) && argumentMap.Keys.containsAll(matchers.Keys);
		if (!containSameKeys)
		{
		  return false;
		}

		foreach (string key in argumentMap.Keys)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: org.hamcrest.Matcher<?> matcher = matchers.get(key);
		  Matcher<object> matcher = matchers[key];
		  object value = null;
		  if (argumentMap is VariableMap)
		  {
			VariableMap varMap = (VariableMap) argumentMap;
			value = varMap.getValueTyped(key);
		  }
		  else
		  {
			value = argumentMap[key];
		  }
		  if (!matcher.matches(value))
		  {
			return false;
		  }
		}


		return true;
	  }

	  public static EqualsMap containsExactly(IDictionary<string, object> map)
	  {
		EqualsMap matcher = new EqualsMap();
		matcher.mapToCompare = map;
		return matcher;
	  }

	  public static EqualsMap matchesExactly<T1>(IDictionary<T1> matchers)
	  {
		EqualsMap matcher = new EqualsMap();
		matcher.matchers = matchers;
		return matcher;
	  }

	  public virtual EqualsMap matcher<T1>(string key, Matcher<T1> matcher)
	  {
		if (matchers == null)
		{
//JAVA TO C# CONVERTER WARNING: Java wildcard generics have no direct equivalent in .NET:
//ORIGINAL LINE: this.matchers = new java.util.HashMap<String, org.hamcrest.Matcher<?>>();
		  this.matchers = new Dictionary<string, Matcher<object>>();
		}

		matchers[key] = matcher;
		return this;
	  }

	}

}