using System;
using System.IO;

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
namespace org.camunda.bpm.engine.variable.impl.value
{

	using FileValueType = org.camunda.bpm.engine.variable.type.FileValueType;
	using ValueType = org.camunda.bpm.engine.variable.type.ValueType;
	using FileValue = org.camunda.bpm.engine.variable.value.FileValue;

	/// <summary>
	/// @author Ronny Bräunlich
	/// @since 7.4
	/// 
	/// </summary>
	[Serializable]
	public class FileValueImpl : FileValue
	{

	  private const long serialVersionUID = 1L;
	  protected internal string mimeType;
	  protected internal string filename;
	  protected internal sbyte[] value;
	  protected internal FileValueType type;
	  protected internal string encoding;
	  protected internal bool isTransient;

	  public FileValueImpl(sbyte[] value, FileValueType type, string filename, string mimeType, string encoding)
	  {
		this.value = value;
		this.type = type;
		this.filename = filename;
		this.mimeType = mimeType;
		this.encoding = encoding;
	  }

	  public FileValueImpl(FileValueType type, string filename) : this(null, type, filename, null, null)
	  {
	  }

	  public virtual string Filename
	  {
		  get
		  {
			return filename;
		  }
	  }

	  public virtual string MimeType
	  {
		  get
		  {
			return mimeType;
		  }
		  set
		  {
			this.mimeType = value;
		  }
	  }


	  public virtual void setValue(sbyte[] bytes)
	  {
		this.value = bytes;
	  }

	  public virtual Stream getValue()
	  {
		if (value == null)
		{
		  return null;
		}
		return new MemoryStream(value);
	  }

	  public virtual ValueType Type
	  {
		  get
		  {
			return type;
		  }
	  }

	  public virtual string Encoding
	  {
		  set
		  {
			this.encoding = value;
		  }
		  get
		  {
			return encoding;
		  }
	  }

	  public virtual Charset Encoding
	  {
		  set
		  {
			this.encoding = value.name();
		  }
	  }

	  public virtual Charset EncodingAsCharset
	  {
		  get
		  {
			if (string.ReferenceEquals(encoding, null))
			{
			  return null;
			}
			return Charset.forName(encoding);
		  }
	  }


	  /// <summary>
	  /// Get the byte array directly without wrapping it inside a stream to evade
	  /// not needed wrapping. This method is intended for the internal API, which
	  /// needs the byte array anyways.
	  /// </summary>
	  public virtual sbyte[] ByteArray
	  {
		  get
		  {
			return value;
		  }
	  }

	  public override string ToString()
	  {
		return "FileValueImpl [mimeType=" + mimeType + ", filename=" + filename + ", type=" + type + ", isTransient=" + isTransient + "]";
	  }

	  public virtual bool Transient
	  {
		  get
		  {
			return isTransient;
		  }
		  set
		  {
			this.isTransient = value;
		  }
	  }

	}

}