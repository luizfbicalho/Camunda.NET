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
namespace org.camunda.bpm.integrationtest.functional.scriptengine.engine
{


	/// <summary>
	/// @author Thorben Lindhauer
	/// 
	/// </summary>
	public abstract class AbstractScriptEngineFactory : ScriptEngineFactory
	{

	  protected internal string name;
	  protected internal string version;
	  protected internal ScriptEngineBehavior behavior;

	  public AbstractScriptEngineFactory(string name, string version, ScriptEngineBehavior behavior)
	  {
		this.name = name;
		this.version = version;
		this.behavior = behavior;
	  }

	  public virtual string EngineName
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string EngineVersion
	  {
		  get
		  {
			return version;
		  }
	  }

	  public virtual IList<string> Extensions
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public virtual IList<string> MimeTypes
	  {
		  get
		  {
			return Collections.emptyList();
		  }
	  }

	  public virtual IList<string> Names
	  {
		  get
		  {
			return Arrays.asList(name);
		  }
	  }

	  public virtual string LanguageName
	  {
		  get
		  {
			return name;
		  }
	  }

	  public virtual string LanguageVersion
	  {
		  get
		  {
			return version;
		  }
	  }

	  public virtual object getParameter(string key)
	  {
		if (key.Equals("THREADING"))
		{
		  return "MULTITHREADED";
		}
		return null;
	  }

	  public virtual string getMethodCallSyntax(string obj, string m, params string[] args)
	  {
		throw new System.NotSupportedException("getMethodCallSyntax");
	  }

	  public virtual string getOutputStatement(string toDisplay)
	  {
		throw new System.NotSupportedException("getOutputStatement");
	  }

	  public virtual string getProgram(params string[] statements)
	  {
		throw new System.NotSupportedException("getProgram");
	  }

	  public virtual ScriptEngine ScriptEngine
	  {
		  get
		  {
			return new AbstractScriptEngineAnonymousInnerClass(this);
		  }
	  }

	  private class AbstractScriptEngineAnonymousInnerClass : AbstractScriptEngine
	  {
		  private readonly AbstractScriptEngineFactory outerInstance;

		  public AbstractScriptEngineAnonymousInnerClass(AbstractScriptEngineFactory outerInstance)
		  {
			  this.outerInstance = outerInstance;
		  }


//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object eval(String script, javax.script.ScriptContext context) throws javax.script.ScriptException
		  public override object eval(string script, ScriptContext context)
		  {
			return outerInstance.behavior.eval(script, context);
		  }

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: @Override public Object eval(java.io.Reader reader, javax.script.ScriptContext context) throws javax.script.ScriptException
		  public override object eval(Reader reader, ScriptContext context)
		  {
			return null;
		  }

		  public override Bindings createBindings()
		  {
			return new SimpleBindings();
		  }

		  public override ScriptEngineFactory Factory
		  {
			  get
			  {
				return outerInstance;
			  }
		  }

	  }


	  public interface ScriptEngineBehavior
	  {
		object eval(string script, ScriptContext context);
	  }
	}

}