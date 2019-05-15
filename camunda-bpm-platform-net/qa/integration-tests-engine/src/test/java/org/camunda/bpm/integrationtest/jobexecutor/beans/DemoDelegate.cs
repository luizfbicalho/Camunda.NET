using System;
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
namespace org.camunda.bpm.integrationtest.jobexecutor.beans
{

	using DelegateExecution = org.camunda.bpm.engine.@delegate.DelegateExecution;
	using Expression = org.camunda.bpm.engine.@delegate.Expression;
	using JavaDelegate = org.camunda.bpm.engine.@delegate.JavaDelegate;

	public class DemoDelegate : JavaDelegate
	{

//JAVA TO C# CONVERTER WARNING: The .NET Type.FullName property will not always yield results identical to the Java Class.getName method:
	  internal Logger log = Logger.getLogger(typeof(DemoDelegate).FullName);
	  internal Expression fail;
	  internal DelegateExecution execution;

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public void execute(org.camunda.bpm.engine.delegate.DelegateExecution execution) throws Exception
	  public virtual void execute(DelegateExecution execution)
	  {
		this.execution = execution;

		log.info("Do Something.");

		insertVariable("stringVar", "Demo-Value");
		insertVariable("longVar", 1L);
		insertVariable("longObjectVar", new long?(1));
		insertVariable("intVar", 1);
		insertVariable("intObjectVar", new int?(1));
		insertVariable("shortVar", (short) 1);
		insertVariable("shortObjectVar", Convert.ToInt16("1"));
		insertVariable("byteVar", (sbyte) 1);
		insertVariable("byteObjectVar", Convert.ToSByte("1"));
		insertVariable("booleanVar", true);
		insertVariable("booleanObjectVar", true);
		insertVariable("floatVar", 1.5f);
		insertVariable("floatObjectVar", new float?(1.5f));
		insertVariable("doubleVar", 1.5d);
		insertVariable("doubleObjectVar", new double?(1.5d));
		insertVariable("charVar", 'a');
		insertVariable("charObjectVar", new char?('a'));
		insertVariable("dateObjectVar", DateTime.Now);
		insertVariable("nullable", null);
		insertVariable("random", (new double?(GlobalRandom.NextDouble * 100)).intValue());

		char[] charArray = new char[] {'a', 'b', 'c', 'D'};
		insertVariable("charArrayVar", charArray);
		char?[] characterObjectArray = new char?[]
		{
			new char?('a'),
			new char?('b'),
			new char?('c'),
			new char?('D')
		};
		insertVariable("characterObjectArray", characterObjectArray);

		string byteString = "mycooltextcontentasbyteyesyes!!!";
		insertVariable("byteArrayVar", byteString.GetBytes(Encoding.UTF8));
		sbyte?[] ByteArray = new sbyte?[byteString.Length];
		sbyte[] bytes = byteString.GetBytes(Encoding.UTF8);

		for (int i = 0; i < bytes.Length; i++)
		{
		  sbyte b = bytes[i];
		  ByteArray[i] = new sbyte?(b);
		}
		insertVariable("ByteArrayVariable", ByteArray);

		DemoVariableClass demoVariableClass = new DemoVariableClass();

		demoVariableClass.BooleanObjectProperty = new bool?(true);
		demoVariableClass.BooleanProperty = false;
		demoVariableClass.ByteObjectProperty = new sbyte?(sbyte.MaxValue);
		demoVariableClass.CharProperty = 'z';
		demoVariableClass.DoubleObjectProperty = new double?(2.25d);
		demoVariableClass.DoubleProperty = 1.75d;
		demoVariableClass.FloatObjectProperty = new float?(4.34f);
		demoVariableClass.FloatProperty = 100.005f;
		demoVariableClass.IntArrayProperty = new int[]{1, 2, 3};
		demoVariableClass.IntegerObjectProperty = null;
		demoVariableClass.IntProperty = -10;
		demoVariableClass.LongObjectProperty = new long?(long.MinValue);
		demoVariableClass.LongProperty = long.MaxValue;

		Dictionary<object, object> demoHashMap = new Dictionary<object, object>();
		demoHashMap["key1"] = "value1";
		demoHashMap["key2"] = "value2";
		demoVariableClass.MapProperty = demoHashMap;

		demoVariableClass.ShortObjectProperty = new short?(short.MaxValue);
		demoVariableClass.ShortProperty = short.MinValue;
		demoVariableClass.StringProperty = "cockpit rulez";

		insertVariable("demoVariableClass", demoVariableClass);

		if (null != fail)
		{
		  string failString = (string) fail.getValue(execution);
		  if (null != failString && failString.Equals("true"))
		  {
			log.info("I'm failing now!.");
			throw new Exception("I'm supposed to fail" + (new Random()).Next(5));
		  }
		}
	  }

	  private void insertVariable(string varName, object value)
	  {
		execution.setVariable(varName + (new double?(GlobalRandom.NextDouble * 10)).intValue(), value);
	  }
	}
}